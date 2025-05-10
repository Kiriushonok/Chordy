import { useAuth } from "../../context/AuthContext";
import { useEffect, useState } from "react";
import SongAuthorLink from "../../components/SongAuthorLink/SongAuthorLink";
import "./Profile.css";

const API_BASE_URL = "https://localhost:7007";

const Profile = () => {
    const { user, loading, logout } = useAuth();
    const [activeTab, setActiveTab] = useState("my");
    const [mySongs, setMySongs] = useState([]);
    const [favSongs, setFavSongs] = useState([]);
    const [songsLoading, setSongsLoading] = useState(true);

    useEffect(() => {
        if (!user) return;
        setSongsLoading(true);
        if (activeTab === "my") {
            fetch(`${API_BASE_URL}/api/songs/by-user/${user.id}`)
                .then(res => res.json())
                .then(data => setMySongs(data))
                .finally(() => setSongsLoading(false));
        } else {
            fetch(`${API_BASE_URL}/api/songs/favorites`, { credentials: "include" })
                .then(res => res.json())
                .then(data => setFavSongs(data))
                .finally(() => setSongsLoading(false));
        }
    }, [user, activeTab]);

    if (loading) return <div>Загрузка...</div>;
    if (!user) return <div>Вы не авторизованы</div>;

    return (
        <div className="profile-main-info">
            <div className="profile-header">
                <div className="profile-greeting">
                    <span className="profile-hello">Привет, <b>{user.login}</b>!</span>
                    <button className="profile-logout-btn" onClick={logout}>Выйти</button>
                </div>
            </div>
            <div className="profile-tabs">
                <button
                    className={`profile-tab-btn${activeTab === "my" ? " active" : ""}`}
                    onClick={() => setActiveTab("my")}
                >
                    Мои подборы
                </button>
                <button
                    className={`profile-tab-btn${activeTab === "fav" ? " active" : ""}`}
                    onClick={() => setActiveTab("fav")}
                >
                    Избранное
                </button>
            </div>
            <div className="profile-list">
                {songsLoading ? (
                    <div>Загрузка песен...</div>
                ) : (
                    (activeTab === "my" ? mySongs : favSongs).length === 0 ? (
                        <div className="no-songs">Нет песен</div>
                    ) : (
                        (activeTab === "my" ? mySongs : favSongs).map(song => (
                            <div key={song.id} className="profile-list-item">
                                <SongAuthorLink
                                    song={song}
                                    authors={song.authors}
                                    showSongLink={true}
                                    showAuthorLink={true}
                                    views={song.views}
                                    showViews={true}
                                    date={song.date}
                                    showDate={true}
                                    showFavourites={true}
                                    favouritesCount={song.favouritesCount}
                                />
                            </div>
                        ))
                    )
                )}
            </div>
        </div>
    );
};

export default Profile;