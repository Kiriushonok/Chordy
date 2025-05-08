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
        </div>
      </div>
    );
};
export default SongAuthorLink;