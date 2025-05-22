import React, { useEffect, useState, useRef } from "react";
import "./Song.css";
import ПрокруткаПлюс from "../../assets/img/ПрокруткаПлюс.png";
import ПрокруткаМинус from "../../assets/img/ПрокруткаМинус.png";
import ШрифтПлюс from "../../assets/img/ШрифтПлюс.png";
import ШрифтМинус from "../../assets/img/ШрифтМинус.png";
import ChordFingeringBlock from "../../components/ChordFingeringBlock";
import { useAuth } from "../../context/AuthContext";
import ChordEditorModal from "../../components/ChordEditingModal/ChordEditorModal";
import fetchWithRefresh from '../../utils/fetchWithRefresh';
import { useNavigate, useLocation } from "react-router-dom";
import AddSong from "../addSong/AddSong";

const API_BASE_URL = "https://localhost:7007";

const Song = () => {
  const { user, loading: authLoading } = useAuth();
  const navigate = useNavigate();
  const location = useLocation();
  const [song, setSong] = useState(null);
  const [chordVariationsByChord, setChordVariationsByChord] = useState({});
  const [selectedVariationIdx, setSelectedVariationIdx] = useState({});
  const [allChords, setAllChords] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState("");
  const [fontSize, setFontSize] = useState(16);
  const [scrollSpeed, setScrollSpeed] = useState(0);
  const [chordEditorOpen, setChordEditorOpen] = useState(false);
  const [editingVariation, setEditingVariation] = useState(null);
  const [chordError, setChordError] = useState("");
  const [variationError, setVariationError] = useState("");
  const scrollIntervalRef = useRef(null);
  const scrollPauseTimeoutRef = useRef(null);
  const [isPausedByUser, setIsPausedByUser] = useState(false);
  const [isFavourite, setIsFavourite] = useState(false);
  const [favouritesCount, setFavouritesCount] = useState(0);

  // Маппинг уровня скорости в px/шаг (заметная разница между 1 и 2)
  const speedPxMap = [0, 0.5, 1.2, 2.2, 3.2, 4.5, 6, 7.5, 9, 11, 13];

  // Здесь id можно получить из useParams, если страница подключена к роутеру
  // const { id } = useParams();
  const id = window.location.pathname.split("/").pop();

  // Загружаем все аккорды при монтировании
  useEffect(() => {
    fetch(`${API_BASE_URL}/api/chords`)
      .then(res => res.json())
      .then(setAllChords)
      .catch(() => setAllChords([]));
  }, []);

  useEffect(() => {
    const fetchSongAndVariations = async () => {
      setLoading(true);
      try {
        const res = await fetch(`${API_BASE_URL}/api/songs/${id}`, {
          credentials: 'include'
        });
        if (!res.ok) throw new Error("Ошибка загрузки песни");
        const data = await res.json();
        // Загружаем вариации аккордов одним запросом
        let chordsArr = data.chords && data.chords.length > 0 ? data.chords : null;
        let variationsObj = {};
        let selectedIdxObj = {};
        if (chordsArr) {
          const variationsRes = await fetch(`${API_BASE_URL}/api/chord-variations/by-song/${id}`);
          if (!variationsRes.ok) throw new Error("Ошибка загрузки вариаций аккордов");
          const variationsData = await variationsRes.json();
          chordsArr.forEach(chord => {
            variationsObj[chord.name] = variationsData[chord.id] || [];
            selectedIdxObj[chord.name] = 0;
          });
          // --- Автоматически выделяем аккорды из текста ---
          const text = data.text || "";
          const allChordNames = allChords.map(c => c.name);
          const lines = text.split(/\r?\n/);
          const foundChords = new Set();
          lines.forEach(line => {
            const trimmed = line.trim();
            if (!trimmed) return;
            const words = trimmed.split(/\s+/);
            if (words.length > 0 && words.every(w => allChordNames.includes(w))) {
              words.forEach(w => foundChords.add(w));
            }
          });
          // Добавляем аккорды, которых нет в song.chords
          foundChords.forEach(chordName => {
            if (!chordsArr.some(c => c.name === chordName)) {
              const chordObj = allChords.find(c => c.name === chordName);
              if (chordObj) {
                chordsArr.push({ id: chordObj.id, name: chordObj.name });
                variationsObj[chordObj.name] = [];
                selectedIdxObj[chordObj.name] = 0;
              }
            }
          });
        } else {
          // Если song.chords нет, собираем из вариаций
          const variationsRes = await fetch(`${API_BASE_URL}/api/chord-variations/by-song/${id}`);
          if (!variationsRes.ok) throw new Error("Ошибка загрузки вариаций аккордов");
          const variationsData = await variationsRes.json();
          chordsArr = Object.keys(variationsData).map(chordId => {
            const firstVar = variationsData[chordId][0];
            const chordName = firstVar && firstVar.chordName ? firstVar.chordName : chordId;
            return { id: Number(chordId), name: chordName };
          });
          chordsArr.forEach(chord => {
            variationsObj[chord.name] = variationsData[chord.id] || [];
            selectedIdxObj[chord.name] = 0;
          });
          // --- Автоматически выделяем аккорды из текста ---
          const text = data.text || "";
          const allChordNames = allChords.map(c => c.name);
          const lines = text.split(/\r?\n/);
          const foundChords = new Set();
          lines.forEach(line => {
            const trimmed = line.trim();
            if (!trimmed) return;
            const words = trimmed.split(/\s+/);
            if (words.length > 0 && words.every(w => allChordNames.includes(w))) {
              words.forEach(w => foundChords.add(w));
            }
          });
          foundChords.forEach(chordName => {
            if (!chordsArr.some(c => c.name === chordName)) {
              const chordObj = allChords.find(c => c.name === chordName);
              if (chordObj) {
                chordsArr.push({ id: chordObj.id, name: chordObj.name });
                variationsObj[chordObj.name] = [];
                selectedIdxObj[chordObj.name] = 0;
              }
            }
          });
        }
        setSong({ ...data, chords: chordsArr });
        setChordVariationsByChord(variationsObj);
        setSelectedVariationIdx(selectedIdxObj);
      } catch (e) {
        setError(e.message);
      } finally {
        setLoading(false);
      }
    };
    if (allChords.length > 0) {
      fetchSongAndVariations();
    }
  }, [id, allChords]);

  // Подгружаем вариации для аккордов, которых нет в chordVariationsByChord
  useEffect(() => {
    if (!song || !song.chords) return;
    song.chords.forEach(async (chord) => {
      if (!chordVariationsByChord[chord.name] || chordVariationsByChord[chord.name].length === 0) {
        // Подгружаем вариации для этого аккорда
        try {
          const res = await fetch(`${API_BASE_URL}/api/chord-variations/by-chord/${chord.id}`);
          if (res.ok) {
            const variations = await res.json();
            if (variations.length > 0) {
              setChordVariationsByChord(prev => ({
                ...prev,
                [chord.name]: variations
              }));
              setSelectedVariationIdx(prev => ({
                ...prev,
                [chord.name]: 0
              }));
            }
          }
        } catch { }
      }
    });
  }, [song, chordVariationsByChord]);

  // Получаем избранное пользователя и обновляем состояние
  useEffect(() => {
    if (!user || !song) return;
    fetch(`${API_BASE_URL}/api/songs/favorites`, { credentials: "include" })
      .then(res => res.json())
      .then(favs => {
        setIsFavourite(favs.some(s => s.id === song.id));
      });
  }, [user, song]);

  // Обновляем счётчик избранного при загрузке песни
  useEffect(() => {
    if (song && typeof song.favouritesCount === 'number') {
      setFavouritesCount(song.favouritesCount);
    }
  }, [song]);

  const handleFontPlus = () => setFontSize(f => Math.min(f + 2, 32));
  const handleFontMinus = () => setFontSize(f => Math.max(f - 2, 10));

  const handleAddChord = async () => {
    if (authLoading) return;
    if (!user) {
      navigate("/login", { state: { from: window.location.pathname, action: "add-chord" }, replace: true });
      return;
    }
    try {
      await fetchWithRefresh(`${API_BASE_URL}/api/users/me`);
      setChordEditorOpen(true);
    } catch (e) {
      if (e.message === "Failed to refresh token") {
        navigate("/login", { state: { from: window.location.pathname, action: "add-chord" }, replace: true });
      }
    }
  };

  const handleEditVariation = async (variation, chordName) => {
    if (authLoading) return;
    if (!user) {
      navigate("/login", { state: { from: window.location.pathname, action: "edit-chord-variation" }, replace: true });
      return;
    }
    try {
      await fetchWithRefresh(`${API_BASE_URL}/api/users/me`);
      setEditingVariation({ ...variation, chordName });
      setChordEditorOpen(true);
    } catch (e) {
      if (e.message === "Failed to refresh token") {
        navigate("/login", { state: { from: window.location.pathname, action: "edit-chord-variation" }, replace: true });
      }
    }
  };

  const handleDeleteVariation = async (variationId, chordName) => {
    await fetchWithRefresh(`${API_BASE_URL}/api/chord-variations/${variationId}`, {
      method: 'DELETE',
      credentials: 'include',
    });
    setChordVariationsByChord(prev => ({
      ...prev,
      [chordName]: prev[chordName].filter(v => v.id !== variationId)
    }));
    setSelectedVariationIdx(prev => {
      const arr = (prev[chordName] || 0);
      return {
        ...prev,
        [chordName]: Math.max(0, arr - 1)
      };
    });
  };

  // Добавим обработчики для навигации по вариациям аккордов
  const handlePrevVariation = (chordName) => {
    setSelectedVariationIdx(prev => {
      const max = (chordVariationsByChord[chordName] || []).length;
      if (!max) return prev;
      return {
        ...prev,
        [chordName]: (prev[chordName] - 1 + max) % max
      };
    });
  };
  const handleNextVariation = (chordName) => {
    setSelectedVariationIdx(prev => {
      const max = (chordVariationsByChord[chordName] || []).length;
      if (!max) return prev;
      return {
        ...prev,
        [chordName]: (prev[chordName] + 1) % max
      };
    });
  };

  // Функция для подсветки аккордов в тексте
  function highlightChordsInText(text, chordNames) {
    if (!text) return null;
    const lines = text.split('\n');
    let lastChords = "";
    const result = [];
    lines.forEach((line, i) => {
      const words = line.trim().split(/\s+/).filter(Boolean);
      const isChordLine = words.length > 0 && words.every(w => chordNames.includes(w));
      if (isChordLine) {
        lastChords = line;
      } else {
        // строка аккордов (или пустая)
        result.push(
          <div key={i + '-chords'} className="song-chords-line">{lastChords}</div>
        );
        // строка текста
        result.push(
          <div key={i + '-text'} className="song-text-line">{line}</div>
        );
        lastChords = "";
      }
    });
    return result;
  }

  // Сохранение новой вариации аккорда
  const handleChordSave = async (data) => {
    setChordError("");
    setVariationError("");
    const { chordName, applicatura, startFret, bare, fingeringSVG } = data;
    if (!chordName || chordName.trim().length === 0) {
      setChordError("Имя аккорда обязательно");
      return;
    }
    let chordId = null;
    let createdChord = null;
    try {
      // 1. Найти или создать аккорд
      const res = await fetchWithRefresh(`${API_BASE_URL}/api/chords/by-name/${encodeURIComponent(chordName)}`);
      if (res.ok) {
        const chord = await res.json();
        chordId = chord.id;
      } else if (res.status === 404) {
        const createRes = await fetchWithRefresh(`${API_BASE_URL}/api/chords`, {
          method: "POST",
          headers: { "Content-Type": "application/json" },
          credentials: "include",
          body: JSON.stringify({ name: chordName })
        });
        if (!createRes.ok) {
          const err = await createRes.json().catch(() => ({}));
          setChordError(err?.errors?.Name?.join(". ") || err?.detail || "Ошибка при создании аккорда");
          return;
        }
        createdChord = await createRes.json();
        chordId = createdChord.id;
        setAllChords(prev => [...prev, createdChord]);
      } else {
        setChordError("Ошибка при поиске аккорда");
        return;
      }
    } catch (e) {
      if (e.message === "Failed to refresh token") {
        navigate("/login", {
          state: {
            from: location.pathname,
            action: "add-chord",
            draft: { chordName, applicatura, startFret, bare, fingeringSVG }
          },
          replace: true
        });
      } else {
        setChordError("Ошибка при поиске/создании аккорда");
      }
      return;
    }
    // 2. Сохраняем вариацию аккорда
    const variationPayload = {
      chordId,
      applicatura,
      startFret,
      bare,
      fingeringSVG,
    };
    try {
      const res = await fetchWithRefresh(`${API_BASE_URL}/api/chord-variations`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify(variationPayload),
      });
      if (!res.ok) {
        const err = await res.json().catch(() => ({}));
        setVariationError(err?.detail || err?.message || "Ошибка при сохранении вариации аккорда");
        return;
      }
      // 3. После успешного создания — подгружаем актуальные вариации для этого аккорда
      const chord = createdChord || allChords.find(c => c.name === chordName);
      if (chord) {
        try {
          const res = await fetch(`${API_BASE_URL}/api/chord-variations/by-chord/${chord.id}`);
          if (res.ok) {
            const variations = await res.json();
            setChordVariationsByChord(prev => ({
              ...prev,
              [chordName]: variations
            }));
            setSelectedVariationIdx(prev => ({
              ...prev,
              [chordName]: variations.length - 1
            }));
            // Добавляем аккорд в song.chords, если его там нет
            setSong(prev => {
              if (!prev.chords.some(c => c.id === chord.id)) {
                return { ...prev, chords: [...prev.chords, { id: chord.id, name: chord.name }] };
              }
              return prev;
            });
          }
        } catch { }
      }
      setChordEditorOpen(false);
      setEditingVariation(null);
    } catch (e) {
      if (e.message === "Failed to refresh token") {
        navigate("/login", {
          state: {
            from: location.pathname,
            action: "add-chord-variation",
            draft: { chordId, applicatura, startFret, bare, fingeringSVG }
          },
          replace: true
        });
      } else {
        setVariationError("Ошибка при сохранении вариации аккорда");
      }
    }
  };

  // Обновление вариации аккорда
  const handleUpdateVariation = async (data) => {
    setChordError("");
    setVariationError("");
    const { chordName, applicatura, startFret, bare, fingeringSVG } = data;
    try {
      const res = await fetchWithRefresh(`${API_BASE_URL}/api/chord-variations/${editingVariation.id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        credentials: 'include',
        body: JSON.stringify({
          chordId: editingVariation.chordId,
          applicatura,
          startFret,
          bare,
          fingeringSVG,
        })
      });
      if (!res.ok) {
        const err = await res.json().catch(() => ({}));
        setVariationError(err?.detail || err?.message || 'Ошибка при обновлении вариации аккорда');
        return;
      }
      // После успешного обновления — обновляем вариации для этого аккорда с сервера
      const chord = allChords.find(c => c.name === chordName);
      if (chord) {
        try {
          const res = await fetch(`${API_BASE_URL}/api/chord-variations/by-chord/${chord.id}`);
          if (res.ok) {
            const variations = await res.json();
            setChordVariationsByChord(prev => ({
              ...prev,
              [chordName]: variations
            }));
          }
        } catch { }
      }
      setEditingVariation(null);
      setChordEditorOpen(false);
      setVariationError("");
    } catch (e) {
      setVariationError('Ошибка при обновлении вариации аккорда');
    }
  };

  // Автоматический скролл страницы
  useEffect(() => {
    if (scrollIntervalRef.current) {
      clearInterval(scrollIntervalRef.current);
      scrollIntervalRef.current = null;
    }
    if (scrollSpeed > 0 && !isPausedByUser) {
      scrollIntervalRef.current = setInterval(() => {
        window.scrollBy({ top: speedPxMap[scrollSpeed], left: 0, behavior: 'smooth' });
      }, 50);
    }
    return () => {
      if (scrollIntervalRef.current) {
        clearInterval(scrollIntervalRef.current);
        scrollIntervalRef.current = null;
      }
    };
  }, [scrollSpeed, isPausedByUser]);

  // Отключение автоскролла при ручном скролле
  useEffect(() => {
    const onUserScroll = () => {
      if (scrollSpeed > 0) {
        setIsPausedByUser(true);
        if (scrollPauseTimeoutRef.current) clearTimeout(scrollPauseTimeoutRef.current);
        scrollPauseTimeoutRef.current = setTimeout(() => {
          setIsPausedByUser(false);
        }, 2500);
      }
    };
    window.addEventListener('wheel', onUserScroll, { passive: true });
    window.addEventListener('touchmove', onUserScroll, { passive: true });
    return () => {
      window.removeEventListener('wheel', onUserScroll);
      window.removeEventListener('touchmove', onUserScroll);
      if (scrollPauseTimeoutRef.current) clearTimeout(scrollPauseTimeoutRef.current);
    };
  }, [scrollSpeed]);

  const handleScrollPlus = () => {
    if (isPausedByUser) setIsPausedByUser(false);
    setScrollSpeed(s => Math.min(s + 1, 10));
  };
  const handleScrollMinus = () => {
    if (isPausedByUser) setIsPausedByUser(false);
    setScrollSpeed(s => Math.max(s - 1, 0));
  };
  const handleScrollToggle = () => setScrollSpeed(s => s > 0 ? 0 : 1);

  useEffect(() => {
    if (chordEditorOpen && scrollSpeed > 0) setScrollSpeed(0);
  }, [chordEditorOpen]);

  useEffect(() => {
    if (!location.pathname.startsWith("/songs")) {
      setChordEditorOpen(false);
      setEditingVariation(null);
    }
  }, [location.pathname]);

  const handleAddFavourite = async () => {
    if (authLoading) return;
    if (!user) {
      navigate("/login", { state: { from: window.location.pathname }, replace: true });
      return;
    }
    try {
      await fetchWithRefresh(`${API_BASE_URL}/api/users/me`);
      await fetchWithRefresh(`${API_BASE_URL}/api/songs/${song.id}/favorite`, {
        method: "POST",
        credentials: "include"
      });
      setIsFavourite(true);
    } catch (e) {
      if (e.message === "Failed to refresh token") {
        navigate("/login", { state: { from: window.location.pathname }, replace: true });
        return;
      }
    }
  };
  const handleRemoveFavourite = async () => {
    if (authLoading) return;
    if (!user) {
      navigate("/login", { state: { from: window.location.pathname }, replace: true });
      return;
    }
    try {
      await fetchWithRefresh(`${API_BASE_URL}/api/users/me`);
      await fetchWithRefresh(`${API_BASE_URL}/api/songs/${song.id}/favorite`, {
        method: "DELETE",
        credentials: "include"
      });
      setIsFavourite(false);
    } catch (e) {
      if (e.message === "Failed to refresh token") {
        navigate("/login", { state: { from: window.location.pathname }, replace: true });
        return;
      }
    }
  };

  // Проверка: является ли текущий пользователь создателем песни
  const isCreator = user && song && song.userId === user.id;

  // Обработчик удаления песни
  const handleDeleteSong = async () => {
    if (!window.confirm("Вы уверены, что хотите удалить песню?")) return;
    try {
      await fetchWithRefresh(`${API_BASE_URL}/api/songs/${song.id}`, {
        method: "DELETE",
        credentials: "include"
      });
      navigate("/");
    } catch (e) {
      alert("Ошибка при удалении песни");
    }
  };

  // Обработчик редактирования песни
  const handleEditSong = () => {
    navigate("/add-song", { state: { song } });
  };

  if (loading) return <div className="song-main">Загрузка...</div>;
  if (error) return <div className="song-main song-error">{error}</div>;
  if (!song) return null;

  // Формируем строку автора и названия
  let authorTitle = "";
  if (song.authors && song.authors.length > 0) {
    authorTitle = song.authors.map(a => a.name).join(", ") + " - ";
  }

  return (
    <div className="song-main song-layout">
      <div className="song-left">
        <div className="song-title-block">
          <div className="song-title">{authorTitle}{song.name}</div>
        </div>
        <div className="song-controls">
          <div className="song-control-group">
            <img src={ПрокруткаМинус} alt="Прокрутка минус" className="song-control-btn" onClick={handleScrollMinus} />
            {scrollSpeed > 0 ? (
              <span
                className="song-scroll-stop-label"
                onClick={() => setScrollSpeed(0)}
                title="Остановить автопрокрутку"
              >Стоп</span>
            ) : (
              <span style={{ fontWeight: 500, margin: '0 8px' }}>Прокрутка</span>
            )}
            <img src={ПрокруткаПлюс} alt="Прокрутка плюс" className="song-control-btn" onClick={handleScrollPlus} />
            <span style={{ marginLeft: 8, color: '#888', fontSize: 14 }}>{scrollSpeed > 0 ? `Скорость: ${scrollSpeed}` : ''}{isPausedByUser && scrollSpeed > 0 ? ' (Пауза)' : ''}</span>
          </div>
          <div className="song-control-group">
            <img src={ШрифтМинус} alt="Шрифт минус" className="song-control-btn" onClick={handleFontMinus} />
            <span>Шрифт</span>
            <img src={ШрифтПлюс} alt="Шрифт плюс" className="song-control-btn" onClick={handleFontPlus} />
          </div>
        </div>
        <div
          className="song-textarea-view"
          style={{ fontSize, fontFamily: "monospace", whiteSpace: "pre" }}
        >
          {highlightChordsInText(song.text, song.chords.map(c => c.name))}
        </div>
      </div>
      <div className="song-right">
        <div className="add-song-controls song-chords-box">
          <div className="add-song-chords-block">
            {song.chords && song.chords.length > 0 && (
              <div className="add-song-chords-list">
                {(() => {
                  // Определяем, какие аккорды реально встречаются в тексте
                  const text = song.text || "";
                  const chordNamesInText = new Set();
                  const allChordNames = song.chords.map(c => c.name);
                  const lines = text.split(/\r?\n/);
                  lines.forEach(line => {
                    const words = line.trim().split(/\s+/);
                    words.forEach(w => {
                      if (allChordNames.includes(w)) chordNamesInText.add(w);
                    });
                  });
                  return song.chords
                    .filter(chord => chordNamesInText.has(chord.name))
                    .map(chord => {
                      const variations = chordVariationsByChord[chord.name] || [];
                      const idx = selectedVariationIdx[chord.name] || 0;
                      const chordName = variations[idx]?.chordName || chord.name;
                      return (
                        <ChordFingeringBlock
                          key={chord.id}
                          chord={{ ...chord, name: chordName }}
                          variations={variations}
                          idx={idx}
                          onPrev={() => handlePrevVariation(chord.name)}
                          onNext={() => handleNextVariation(chord.name)}
                          onEdit={(variation, chordName) => handleEditVariation(variation, chordName)}
                          onDelete={handleDeleteVariation}
                          user={user}
                          currentUserId={user?.id}
                        />
                      );
                    });
                })()}
              </div>
            )}
          </div>
          <button type="button" className="add-song-btn" onClick={() => handleAddChord()} disabled={authLoading}>Добавить аккорд</button>
          <button
            className="add-song-btn"
            style={{ background: isFavourite ? 'rgb(255 241 184)' : '#fff', color: isFavourite ? '#222' : '#222', borderColor: isFavourite ? '#ffe066' : '#bdbdbd' }}
            onClick={isFavourite ? handleRemoveFavourite : handleAddFavourite}
            disabled={authLoading}
          >
            {isFavourite ? 'Удалить из избранного' : 'Добавить в избранное'}
          </button>
          {isCreator && (
            <>
              <button type="button" className="add-song-btn" onClick={handleEditSong}>Редактировать</button>
              <button type="button" className="add-song-btn" onClick={handleDeleteSong} style={{color: '#d32f2f', borderColor: '#d32f2f'}}>Удалить</button>
            </>
          )}
        </div>
      </div>
      <ChordEditorModal
        open={chordEditorOpen && !!user}
        onClose={() => { setChordEditorOpen(false); setEditingVariation(null); }}
        onSave={editingVariation ? handleUpdateVariation : handleChordSave}
        chordError={chordError}
        variationError={variationError}
        initialData={editingVariation}
      />
    </div>
  );
};

export default Song; 