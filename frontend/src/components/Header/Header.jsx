import userIcon from "../../assets/img/user.svg"
import searchIcon from "../../assets/img/search.svg"
import "./Header.css"
import { useState, useRef, useEffect } from "react";
import { NavLink } from "react-router-dom";

const Header = () => {
    const [collections, setCollections] = useState([]);
    const [dropdownOpen, setDropdownOpen] = useState(false);
    const dropDownRef = useRef(null);

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
                <NavLink to="/login">
                    <button className="header__user-btn">
                        <img src={userIcon} alt="Профиль"></img>
                    </button>
                </NavLink>

                <div className="header__search-box">
                    <input 
                    type="text"
                    placeholder="Поиск"
                    className="header__search-input"
                    >
                    </input>
                    <button className="header__search-btn">
                        <img src={searchIcon} alt="Поиск"></img>
                    </button>
                </div>
            </div>
        </header>
    );
};
export default Header;