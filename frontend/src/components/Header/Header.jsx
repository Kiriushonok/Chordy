import userIcon from "../../assets/img/user.svg"
import searchIcon from "../../assets/img/search.svg"
import "./Header.css"
import { useState, useRef, useEffect } from "react";
import { NavLink, useNavigate } from "react-router-dom";
import { useAuth } from "../../context/AuthContext";

const Header = () => {
    const [collections, setCollections] = useState([]);
    const [dropdownOpen, setDropdownOpen] = useState(false);
    const dropDownRef = useRef(null);
    const [search, setSearch] = useState("");
    const navigate = useNavigate();
    const { user, loading } = useAuth();

    useEffect(() => {
        fetch("https://localhost:7007/api/collections")
        .then(response => response.json())
        .then(data => setCollections(data))
        .catch(error => console.error("Ошибка при получении подборок:", error));
    }, []);

    useEffect(() => {
        if (!dropdownOpen) return;
    
        function handleClickOutside(event) {
            if (dropDownRef.current && !dropDownRef.current.contains(event.target)) {
                setDropdownOpen(false);
            }
        }
        document.addEventListener("mousedown", handleClickOutside);
    
        return () => {
            document.removeEventListener("mousedown", handleClickOutside);
        };
    }, [dropdownOpen]);

    return (
        <header className="header">
            <nav className="header__nav">
                <NavLink to="/" 
                    className={({isActive}) => isActive ? "header__link nav-link--active" : "header__link"} 
                >Главная
                </NavLink>
                <NavLink to="/artists" className={({isActive}) => isActive ? "header__link nav-link--active" : "header__link"} 
                >Исполнители</NavLink>
                <div className="header__dropdown" ref={dropDownRef}>
                    <button 
                        className="header__link header__dropdown-btn"
                        onClick={() => setDropdownOpen((open) => !open)}
                        type="button"
                    >
                        Подборки <span className="header__dropdown-arrow">&#9660;</span>
                    </button>
                    {dropdownOpen && (
                        <ul className="header__dropdown-list">
                            {collections.map((col) => (
                                <li key={col.id}>
                                    <NavLink to={`/collections/${col.id}`}
                                    onClick={() => setDropdownOpen((open) => !open)} 
                                        className={({isActive}) => isActive ? "header__dropdown-item dropwown-item--active" : "header__dropdown-item"} 
                                    >{col.name}
                                    </NavLink>
                                </li>
                            ))}
                        </ul>
                    )}
                </div>
                <NavLink to="/popular" 
                    className={({isActive}) => isActive ? "header__link nav-link--active" : "header__link"} 
                >Популярное</NavLink>
                <NavLink to="/add-song" 
                    className={({isActive}) => isActive ? "header__link nav-link--active" : "header__link"} 
                >Добавить подбор</NavLink>
            </nav>
            <div className="header__functions">
                    <button 
                        className="header__user-btn"
                        onClick={() => {
                            if (loading) return;
                            if (user) {
                                navigate("/profile");
                            } else {
                                navigate("/login", { state: { from: "/profile", directProfile: true } });
                            }
                        }}
                        disabled={loading}
                    >
                        <img src={userIcon} alt="Профиль"></img>
                    </button>

                <form
                    className="header__search-box"
                    onSubmit={e => {
                        e.preventDefault();
                        if (search.trim()) {
                            navigate(`/search?query=${encodeURIComponent(search)}`);
                        }
                    }}
                >
                    <input
                        type="text"
                        placeholder="Поиск"
                        className="header__search-input"
                        value={search}
                        onChange={e => setSearch(e.target.value)}
                    />
                    <button className="header__search-btn" type="submit">
                        <img src={searchIcon} alt="Поиск" />
                    </button>
                </form>
            </div>
        </header>
    );
};
export default Header;