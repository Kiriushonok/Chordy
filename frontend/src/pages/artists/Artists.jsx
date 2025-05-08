import { useEffect, useState } from "react";
import SongAuthorLink from "../../components/SongAuthorLink/SongAuthorLink";
import "./Artists.css";

const API_BASE_URL = "https://localhost:7007";
const PAGE_SIZE = 10;

const Artists = () => {
    const [authors, setAuthors] = useState([]);
    const [expanded, setExpanded] = useState({});
    const [songsByAuthor, setSongsByAuthor] = useState({});
    const [loadingSongs, setLoadingSongs] = useState({});
    const [currentPage, setCurrentPage] = useState(1);
    const [totalCount, setTotalCount] = useState(0);

    useEffect(() => {
        fetch(`${API_BASE_URL}/api/authors/paged?page=${currentPage}&pageSize=${PAGE_SIZE}`)
            .then(res => res.json())
            .then(data => {
                setAuthors(data.items);
                setTotalCount(data.totalCount);
            });
    }, [currentPage]);

    const handleToggle = (authorId) => {
        setExpanded(prev => ({ ...prev, [authorId]: !prev[authorId] }));
        if (!songsByAuthor[authorId] && !loadingSongs[authorId]) {
            setLoadingSongs(prev => ({ ...prev, [authorId]: true }));
            fetch(`${API_BASE_URL}/api/songs/by-author/${authorId}`)
                .then(res => res.json())
                .then(songs => {
                    setSongsByAuthor(prev => ({ ...prev, [authorId]: songs }));
                    setLoadingSongs(prev => ({ ...prev, [authorId]: false }));
                });
        }
    };

    const totalPages = Math.ceil(totalCount / PAGE_SIZE);

    return (
        <div className="artists-main-info">
            <div className="artists-column">
                <h3>Популярные исполнители</h3>
                <div className="artists-list">
                    {authors.map(author => (
                        <div key={author.id} className="artists-list-item">
                            <div className="artist-header" onClick={() => handleToggle(author.id)}>
                                <SongAuthorLink
                                    authors={[author]}
                                    showSongLink={false}
                                    showAuthorLink={true}
                                    views={author.totalViews}
                                    showViews={true}
                                />
                                <span className={`arrow ${expanded[author.id] ? "expanded" : ""}`}>▶</span>
                            </div>
                            {expanded[author.id] && (
                                <div className="artist-songs">
                                    {loadingSongs[author.id] ? (
                                        <div className="loading">Загрузка песен...</div>
                                    ) : (
                                        (songsByAuthor[author.id] && songsByAuthor[author.id].length > 0) ? (
                                            songsByAuthor[author.id].map(song => (
                                                <SongAuthorLink
                                                    key={song.id}
                                                    song={song}
                                                    authors={song.authors}
                                                    showSongLink={true}
                                                    showAuthorLink={false}
                                                    views={song.views}
                                                    showViews={true}
                                                />
                                            ))
                                        ) : (
                                            <div className="no-songs">Нет песен</div>
                                        )
                                    )}
                                </div>
                            )}
                        </div>
                    ))}
                </div>
                {/* Пагинация */}
                {totalPages > 1 && (
                    <div className="pagination">
                        {Array.from({ length: totalPages }, (_, i) => (
                            <button
                                key={i + 1}
                                className={`pagination-btn${currentPage === i + 1 ? " active" : ""}`}
                                onClick={() => setCurrentPage(i + 1)}
                            >
                                {i + 1}
                            </button>
                        ))}
                    </div>
                )}
            </div>
        </div>
    );
};

export default Artists;