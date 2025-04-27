import "./Footer.css";
import { NavLink } from "react-router-dom"

const Footer = () => {
    return (
        <footer className="footer">
            <div className="footer__container">
                <NavLink to="/feedback" 
                    className={({isActive}) => isActive ? "footer__link nav-link--active" : "footer__link"} 
                >Обратная связь</NavLink>
                <NavLink to="/rules" 
                    className={({isActive}) => isActive ? "footer__link nav-link--active" : "footer__link"} 
                >Правила сайта</NavLink>
            </div>
            <div className="footer__copyright">
                © {new Date().getFullYear()} Chordy
            </div>
        </footer>
    )
}
export default Footer;