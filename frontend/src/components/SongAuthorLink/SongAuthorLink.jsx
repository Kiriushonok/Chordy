import "./SongAuthorLink.css";
import { Link } from "react-router-dom";

const API_BASE_URL = "https://localhost:7007";

const SongAuthorLink = ({
  song,
  authors = [],
  showSongLink = true,
  showAuthorLink = true,
  date,
  views,
  showDate = false,
  showViews = false,
  showFavourites = false,
  favouritesCount = 0,
  children,
}) => {
  return (
    <div className="song-author-link">
      <div className="main-info">
        {showAuthorLink && authors.length > 0 && (
          <>
            {authors.map((author, idx) => (
              <span key={author.id} className="author-info">
                <img
                  src={author.avatarPath ? API_BASE_URL + author.avatarPath : "/img/placeholder.png"}
                  alt={author.name}
                  className="author-avatar"
                />
                <Link to={`/authors/${author.id}`} className="author-link">{author.name}</Link>
                {idx < authors.length - 1 && ', '}
              </span>
            ))}
            {showSongLink && song && " - "}
          </>
        )}
        {showSongLink && song && (
          <>
            <Link to={`/songs/${song.id}`} className="song-link">{song.name}</Link>
            {song.userLogin && (
              <span className="song-user-login"> &nbsp; <span>от</span> <span className="user-login">{song.userLogin}</span></span>
            )}
          </>
        )}
        {children}
      </div>
      <div className="side-info">
        {showDate && date && <span className="date">{new Date(date).toLocaleDateString()},</span>}
        {showViews && (
          <div className="views">
            <span className="views-number">{views}</span>
            <svg
              xmlns="http://www.w3.org/2000/svg"
              fill="none"
              viewBox="0 0 24 24"
              strokeWidth="1.5"
              stroke="currentColor"
            >
              <path strokeLinecap="round" strokeLinejoin="round" d="M2.036 12.322a1.012 1.012 0 0 1 0-.639C3.423 7.51 7.36 4.5 12 4.5c4.638 0 8.573 3.007 9.963 7.178.07.207.07.431 0 .639C20.577 16.49 16.64 19.5 12 19.5c-4.638 0-8.573-3.007-9.963-7.178Z" />
              <path strokeLinecap="round" strokeLinejoin="round" d="M15 12a3 3 0 1 1-6 0 3 3 0 0 1 6 0Z" />
            </svg>
          </div>
        )}
        {showFavourites && (
          <div className="favourites">
            <svg className="favourites-icon" viewBox="0 0 24 24" fill="currentColor" xmlns="http://www.w3.org/2000/svg">
              <path d="M8.58737 8.23597L11.1849 3.00376C11.5183 2.33208 12.4817 2.33208 12.8151 3.00376L15.4126 8.23597L21.2215 9.08017C21.9668 9.18848 22.2638 10.0994 21.7243 10.6219L17.5217 14.6918L18.5135 20.4414C18.6409 21.1798 17.8614 21.7428 17.1945 21.3941L12 18.678L6.80547 21.3941C6.1386 21.7428 5.35909 21.1798 5.48645 20.4414L6.47825 14.6918L2.27575 10.6219C1.73617 10.0994 2.03322 9.18848 2.77852 9.08017L8.58737 8.23597Z"
                stroke="#000" stroke-width="0.5" stroke-linecap="round" stroke-linejoin="round" />
            </svg>
            <span className="favourites-number">{favouritesCount}</span>
          </div>
        )}
      </div>
    </div>
  );
};
export default SongAuthorLink;