import { useEffect, useState } from "react";
import SongAuthorLink from "../../components/SongAuthorLink/SongAuthorLink";
import "./Home.css";

const Home = () => {
    const [data, setData] = useState(null);

    useEffect(() => {
        fetch("https://localhost:7007/api/main-info")
        .then(response => response.json())
        .then(setData)
    }, []);

    if (!data) return <div>Загрузка...</div>
    return (
        <div className="home-main-info">
            {/* Популярные подборы */}
            <div className="home-column">
                <h3>Популярные подборы</h3>
                <div className="home-list">
                  {data.popularSongs.map(song => (
                      <div key={song.id} className="home-list-item">
                          <SongAuthorLink
                              song={song}
                              authors={song.authors}
                              showSongLink={true}
                              showAuthorLink={true}
                              views={song.views}
                              showViews={true}
                          />
                      </div>
                  ))}
                </div>
            </div>

            {/* Популярные исполнители */}
            <div className="home-column">
                <h3>Популярные исполнители</h3>
                <div className="home-list">
                  {data.popularAuthors.map(author => (
                      <div key={author.id} className="home-list-item">
                          <SongAuthorLink
                              authors={[author]}
                              showSongLink={false}
                              showAuthorLink={true}
                              views={author.totalViews}
                              showViews={true}
                          />
                      </div>
                  ))}
                </div>
            </div>

            {/* Новые подборы */}
            <div className="home-column">
                <h3>Новые подборы</h3>
                <div className="home-list">
                  {data.newSongs.map(song => (
                      <div key={song.id} className="home-list-item">
                          <SongAuthorLink
                              song={song}
                              authors={song.authors}
                              showSongLink={true}
                              showAuthorLink={true}
                              views={song.views}
                              showViews={true}
                              date={song.date}
                              showDate={true}
                          />
                      </div>
                  ))}
                </div>
            </div>
        </div>
    )
}

export default Home;