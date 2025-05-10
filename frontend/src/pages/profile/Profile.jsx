import { useAuth } from "../../context/AuthContext";

const Profile = () => {
    const { user, loading } = useAuth();

    if (loading) return <div>Загрузка...</div>;
    if (!user) return <div>Вы не авторизованы</div>;

    return (
        <div className="profile-page">
            <h2>Профиль</h2>
            <div><b>Логин:</b> {user.login}</div>
        </div>
    );
};

export default Profile;