import { useState } from "react";
import { useNavigate, useLocation, Link } from "react-router-dom";
import "./Register.css";

const API_BASE_URL = "https://localhost:7007";

const Register = () => {
    const [login, setLogin] = useState("");
    const [password, setPassword] = useState("");
    const [confirmPassword, setConfirmPassword] = useState("");
    const [error, setError] = useState("");
    const [fieldErrors, setFieldErrors] = useState({});
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();
    const location = useLocation();
    const from = location.state?.from;
    const directProfile = location.state?.directProfile;

    // Фронтенд-валидация
    const validate = () => {
        const errors = {};
        if (!login || login.length < 1) {
            errors.login = "Логин не может быть пустым";
        } else if (login.length > 30) {
            errors.login = "Логин не должен превышать 30 символов";
        }
        if (!password || password.length < 8) {
            errors.password = "Пароль должен быть не короче 8 символов";
        } else if (password.length > 64) {
            errors.password = "Пароль не должен превышать 64 символа";
        }
        if (password !== confirmPassword) {
            errors.confirmPassword = "Пароли не совпадают";
        }
        return errors;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");
        setFieldErrors({});
        const errors = validate();
        if (Object.keys(errors).length > 0) {
            setFieldErrors(errors);
            return;
        }
        setLoading(true);
        try {
            const response = await fetch(`${API_BASE_URL}/api/users/register`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                body: JSON.stringify({ login, password })
            });
            if (response.ok) {
                navigate("/login", { state: directProfile ? { from, directProfile: true } : (from ? { from } : undefined) });
            } else if (response.status === 400) {
                const data = await response.json();
                if (data.errors) {
                    const newFieldErrors = {};
                    for (const key in data.errors) {
                        const field = key.toLowerCase();
                        newFieldErrors[field] = data.errors[key][0];
                    }
                    setFieldErrors(newFieldErrors);
                } else if (data.message) {
                    setError(data.message);
                } else {
                    setError("Ошибка валидации");
                }
            } else if (response.status === 409) {
                const data = await response.json();
                setError(data.detail || "Пользователь уже существует");
            } else {
                const data = await response.json().catch(() => ({}));
                setError(data.message || "Ошибка сервера. Попробуйте позже.");
            }
        } catch (err) {
            setError("Ошибка сети. Попробуйте позже.");
        } finally {
            setLoading(false);
        }
    };

    return (
        <div className="register-page">
            <h2 className="register-title">Регистрация</h2>
            <form className="register-form" onSubmit={handleSubmit} noValidate>
                <label className="register-label">
                    Логин:
                    <input
                        type="text"
                        value={login}
                        onChange={e => setLogin(e.target.value)}
                        required
                        autoFocus
                        autoComplete="username"
                        className={fieldErrors.login ? "input-error" : ""}
                    />
                    {fieldErrors.login && <div className="field-error">{fieldErrors.login}</div>}
                </label>
                <label className="register-label">
                    Пароль:
                    <input
                        type="password"
                        value={password}
                        onChange={e => setPassword(e.target.value)}
                        required
                        autoComplete="new-password"
                        className={fieldErrors.password ? "input-error" : ""}
                    />
                    {fieldErrors.password && <div className="field-error">{fieldErrors.password}</div>}
                </label>
                <label className="register-label">
                    Подтвердите пароль:
                    <input
                        type="password"
                        value={confirmPassword}
                        onChange={e => setConfirmPassword(e.target.value)}
                        required
                        autoComplete="new-password"
                        className={fieldErrors.confirmPassword ? "input-error" : ""}
                    />
                    {fieldErrors.confirmPassword && <div className="field-error">{fieldErrors.confirmPassword}</div>}
                </label>
                {error && <div className="register-error">{error}</div>}
                <button type="submit" className="register-btn" disabled={loading}>
                    {loading ? "Регистрация..." : "Зарегистрироваться"}
                </button>
            </form>
            <div className="register-login-link">
                Уже есть аккаунт? <Link to="/login" state={directProfile ? { from, directProfile: true } : (from ? { from } : undefined)}>Войти</Link>
            </div>
        </div>
    );
};

export default Register; 