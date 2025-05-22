import { useLocation } from "react-router-dom";
import { useState, useEffect } from "react";
import SongAuthorLink from "../../components/SongAuthorLink/SongAuthorLink";
import "./SearchResults.css";

const API_BASE_URL = "https://localhost:7007";

const SearchResults = () => {
    const query = new URLSearchParams(useLocation().search).get("query") || "";
    const [songs, setSongs] = useState([]);
    const [authors, setAuthors] = useState([]);
    const [loading, setLoading] = useState(false);

    useEffect(() => {
        if (!query) return;
        setLoading(true);
        Promise.all([
            fetch(`${API_BASE_URL}/api/songs/search?query=${encodeURIComponent(query)}`).then(res => res.json()),
            fetch(`${API_BASE_URL}/api/authors/search?query=${encodeURIComponent(query)}`).then(res => res.json())
        ]).then(([songs, authors]) => {
            setSongs(songs);
            setAuthors(authors);
            setLoading(false);
        });
    }, [query]);

    if (!query) return null;
    if (loading) return <div>Загрузка...</div>;

    return (
        <div className="search-main-info">
            <div className="search-header">
                <span className="search-title">Результаты поиска</span>
            </div>
            <div className="search-list-block">
                <h3 className="search-section-title">Песни</h3>
                <div className="search-list">
                    {songs.length > 0 ? songs.map(song => (
                        <div key={song.id} className="search-list-item">
                            <SongAuthorLink song={song} authors={song.authors} showSongLink={true} showAuthorLink={true} views={song.views} showViews={true} />
                        </div>
                    )) : <div className="no-songs">Нет песен</div>}
                </div>
            </div>
            <div className="search-list-block">
                <h3 className="search-section-title">Авторы</h3>
                <div className="search-list">
                    {authors.length > 0 ? authors.map(author => (
                        <div key={author.id} className="search-list-item">
                            <SongAuthorLink
                                authors={[author]}
                                showSongLink={false}
                                showAuthorLink={true}
                                views={author.totalViews}
                                showViews={true}
                            />
                        </div>
                    )) : <div className="no-songs">Нет авторов</div>}
                </div>
            </div>
        </div>
    );
};

export default SearchResults;
