import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import SongAuthorLink from "../../components/SongAuthorLink/SongAuthorLink";
import "./Collection.css";

const API_BASE_URL = "https://localhost:7007";
const PAGE_SIZE = 15;

const Collection = () => {
    const { collectionId } = useParams();
    const [songs, setSongs] = useState([]);
    const [totalCount, setTotalCount] = useState(0);
    const [currentPage, setCurrentPage] = useState(1);
    const [collection, setCollection] = useState(null);

        // Получаем данные о подборке (название)
        useEffect(() => {
            fetch(`${API_BASE_URL}/api/collections/${collectionId}`)
                .then(res => res.json())
                .then(setCollection);
        }, [collectionId]);

    useEffect(() => {
        fetch(`${API_BASE_URL}/api/songs/by-collection/${collectionId}/paged?page=${currentPage}&pageSize=${PAGE_SIZE}`)
            .then(res => res.json())
            .then(data => {
                setSongs(data.items);
                setTotalCount(data.totalCount);
            });
    }, [collectionId, currentPage]);

    const totalPages = Math.ceil(totalCount / PAGE_SIZE);

    return (
        <div className="collection-main-info">
            <div className="collection-header">
                <span className="collection-title">Песни подборки</span>
                {collection && (
                    <span className="collection-name">{collection.name}</span>
                )}
            </div>
            <div className="collection-list">
                {songs.length > 0 ? (
                    songs.map((song, idx) => (
                        <div key={song.id} className="collection-list-item">
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
                    <div className="no-songs">Нет песен в этой подборке</div>
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
};

export default Collection;