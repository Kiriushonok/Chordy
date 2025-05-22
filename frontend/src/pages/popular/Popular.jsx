import { useEffect, useState } from "react";
import SongAuthorLink from "../../components/SongAuthorLink/SongAuthorLink";
import "./Popular.css";

const API_BASE_URL = "https://localhost:7007";
const PAGE_SIZE = 15;

const Popular = () => {
    const [songs, setSongs] = useState([]);
    const [totalCount, setTotalCount] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);

    useEffect(() => {
        fetch(`${API_BASE_URL}/api/songs/popular?page=${currentPage}&pageSize=${PAGE_SIZE}`)
        .then(res => res.json())
        .then(data => {
            setSongs(data.items);
            setTotalCount(data.totalCount);
        });
    }, [currentPage]);

    const totalPages = Math.ceil(totalCount / PAGE_SIZE);

    return (
        <div className="popular-main-info">
            <div className="popular-header">
                <span className="popular-title">Популярные песни</span>
            </div>
            <div className="popular-list">
                {songs.length > 0 ? (
                    songs.map(song => (
                        <div key={song.id} className="popular-list-item">
                            <SongAuthorLink
                                song={song}
                                authors={song.authors}
                                showSongLink={true}
                                showAuthorLink={true}
                                views={song.views}
                                showViews={true}
                            />
                        </div>
                    ))
                ) : (
                    <div className="no-songs">Нет песен</div>
                )}
            </div>
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
    );
}

export default Popular;