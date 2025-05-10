// frontend/src/components/ProtectedRoute.jsx
import { useEffect, useState } from "react";
import { useAuth } from "../context/AuthContext";
import { Navigate, useLocation } from "react-router-dom";

const ProtectedRoute = ({ children }) => {
    const { user, loading, refreshUser } = useAuth();
    const [checking, setChecking] = useState(true);
    const location = useLocation();

    useEffect(() => {
        // При каждом переходе на защищённый роут — актуализируем user
        const checkAuth = async () => {
            setChecking(true);
            await refreshUser();
            setChecking(false);
        };
        checkAuth();
        // eslint-disable-next-line
    }, [location.pathname]);

    if (loading || checking) {
        return <div>Проверка авторизации...</div>;
    }

    if (!user) {
        return <Navigate to="/login" state={{ from: location }} replace />;
    }

    return children;
};

export default ProtectedRoute;