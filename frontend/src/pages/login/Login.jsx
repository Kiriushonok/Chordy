import { useState, useEffect, useRef } from "react";
import { useNavigate, useLocation, Link } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";
import "./Login.css";

const API_BASE_URL = "https://localhost:7007";

const Login = () => {
    const [login, setLogin] = useState("");
    const [password, setPassword] = useState("");
    const [rememberMe, setRememberMe] = useState(false);
    const [error, setError] = useState("");
    const [fieldErrors, setFieldErrors] = useState({});
    const [loadingLocal, setLoadingLocal] = useState(false);
    const { user, loading, refreshUser } = useAuth();
    const navigate = useNavigate();
    const location = useLocation();
    const from = location.state?.from;
    const directProfile = location.state?.directProfile;
    const defaultRedirect = "/profile";
    const prevUserRef = useRef();
    const [wasLoginSubmit, setWasLoginSubmit] = useState(false);

    useEffect(() => {
        if (!loading && user && (wasLoginSubmit || directProfile)) {
            if (directProfile) {
                navigate(defaultRedirect, { replace: true });
            } else {
                navigate(from || defaultRedirect, { replace: true });
            }
        }
        prevUserRef.current = user;
    }, [loading, user, navigate, from, directProfile, wasLoginSubmit]);

    useEffect(() => {
        refreshUser();
        // eslint-disable-next-line
    }, []);

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
        setLoadingLocal(true);
        try {
            const response = await fetch(`${API_BASE_URL}/api/users/login`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify({ login, password, rememberMe })
            });
            if (response.ok) {
                setWasLoginSubmit(true);
                await refreshUser();
            } else if (response.status === 401) {
                setError("Неверный логин или пароль");
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
            } else {
                const data = await response.json().catch(() => ({}));
                setError(data.message || "Ошибка сервера. Попробуйте позже.");
            }
        } catch (err) {
            setError("Ошибка сети. Попробуйте позже.");
        } finally {
            setLoadingLocal(false);
        }
    };

    if (loading) {
        return <div>Загрузка...</div>;
    }

    return (
        <div className="login-page">
            <h2 className="login-title">Вход</h2>
            <form className="login-form" onSubmit={handleSubmit} noValidate>
                <label className="login-label">
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
                <label className="login-label">
                    Пароль:
                    <input
                        type="password"
                        value={password}
                        onChange={e => setPassword(e.target.value)}
                        required
                        autoComplete="current-password"
                        className={fieldErrors.password ? "input-error" : ""}
                    />
                    {fieldErrors.password && <div className="field-error">{fieldErrors.password}</div>}
                </label>
                <label className="remember-label">
                    <input
                        type="checkbox"
                        checked={rememberMe}
                        onChange={e => setRememberMe(e.target.checked)}
                    />
                    Запомнить меня
                </label>
                {error && <div className="login-error">{error}</div>}
                <button type="submit" className="login-btn" disabled={loadingLocal}>
                    {loadingLocal ? "Вход..." : "Войти"}
                </button>
            </form>
            <div className="login-register-link">
                Ещё нет аккаунта? <Link to="/register" state={directProfile ? { from, directProfile: true } : (from ? { from } : undefined)}>Зарегистрироваться</Link>
            </div>
        </div>
    );
};

export default Login;