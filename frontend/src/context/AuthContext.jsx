import { createContext, useContext, useEffect, useState, useCallback } from "react";
import fetchWithRefresh from "../utils/fetchWithRefresh";

const AuthContext = createContext();

export const useAuth = () => useContext(AuthContext);

export const AuthProvider = ({ children }) => {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    // Получение пользователя с сервера с поддержкой refresh
    const refreshUser = useCallback(async () => {
        setLoading(true);
        try {
            const res = await fetchWithRefresh("https://localhost:7007/api/users/me");
            if (res.ok) {
                const data = await res.json();
                setUser(data);
            } else {
                setUser(null);
            }
        } catch {
            setUser(null);
        } finally {
            setLoading(false);
        }
    }, []);

    useEffect(() => {
        refreshUser();
    }, [refreshUser]);

    const login = async () => {
        setLoading(true);
        try {
            const res = await fetchWithRefresh("https://localhost:7007/api/users/me");
            if (res.ok) {
                const data = await res.json();
                setUser(data);
            } else {
                setUser(null);
            }
        } catch {
            setUser(null);
        } finally {
            setLoading(false);
        }
    };

    const logout = async () => {
        setLoading(true);
        try {
            await fetchWithRefresh("https://localhost:7007/api/users/logout", {
                method: "POST"
            });
            setUser(null);
        } catch {
            setUser(null);
        } finally {
            setLoading(false);
        }
    };

    return (
        <AuthContext.Provider value={{ user, loading, login, logout, refreshUser }}>
            {children}
        </AuthContext.Provider>
    );
};
