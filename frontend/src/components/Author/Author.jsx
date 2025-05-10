import { useState, useEffect } from "react";
import { useParams } from "react-router-dom";
import SongAuthorLink from "../SongAuthorLink/SongAuthorLink";
import "./Author.css";

const API_BASE_URL = "https://localhost:7007";
const PAGE_SIZE = 15;

const Author = () => {
    const { authorId } = useParams();
    const [author, setAuthor] = useState(null);
    const [songs, setSongs] = useState([]);
    const [totalCount, setTotalCount] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);

    useEffect(() => {
        fetch(`${API_BASE_URL}/api/authors/by-id/${authorId}`)
            .then(res => res.json())
            .then(setAuthor);

        fetch(`${API_BASE_URL}/api/songs/by-author/${authorId}/paged?page=${currentPage}&pageSize=${PAGE_SIZE}`)
            .then(res => res.json())
            .then(data => {
                setSongs(data.items);
                setTotalCount(data.totalCount);
            })
    }, [authorId, currentPage]);

    const totalPages = Math.ceil(totalCount / PAGE_SIZE);

    if (!author) return <div>Автор не найден</div>;

    return (
        <div className="author-page">
            <div className="author-header">
                <img
                    src={author.avatarPath ? API_BASE_URL + author.avatarPath : "/img/placeholder.png"}
                    alt={author.name}
                    className="author-avatar-large"
                />
                <div className="author-header-info">
                    <h2>{author.name}</h2>
                    <div className="author-total-views">Всего просмотров: {author.totalViews}</div>
                </div>
            </div>
            <div className="author-songs">
                <h3>Песни автора</h3>
                {songs.length > 0 ? (
                    songs.map(song => (
                        <div key={song.id} className="author-song-item">
                            <SongAuthorLink
                                song={song}
                                authors={[author]}
                                showSongLink={true}
                                showAuthorLink={false}
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
export default Author;