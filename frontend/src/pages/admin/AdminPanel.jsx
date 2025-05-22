import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import ChordFingeringBlock from "../../components/ChordFingeringBlock";
import ChordEditorModal from "../../components/ChordEditingModal/ChordEditorModal";
import fetchWithRefresh from "../../utils/fetchWithRefresh";
import { useAuth } from "../../context/AuthContext";

const sections = [
  { key: "songs", label: "Песни" },
  { key: "collections", label: "Подборки" },
  { key: "authors", label: "Авторы" },
  { key: "chordVariations", label: "Вариации аккордов" },
  { key: "chords", label: "Аккорды" },
];

const API_BASE_URL = "https://localhost:7007";

export default function AdminPanel() {
  const [activeSection, setActiveSection] = useState("songs");
  const [songs, setSongs] = useState([]);
  const [collections, setCollections] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const [showCollectionModal, setShowCollectionModal] = useState(false);
  const [collectionModalMode, setCollectionModalMode] = useState("add"); // add | edit
  const [collectionModalValue, setCollectionModalValue] = useState("");
  const [editingCollectionId, setEditingCollectionId] = useState(null);
  const [collectionModalError, setCollectionModalError] = useState("");
  const [authors, setAuthors] = useState([]);
  const [showAuthorModal, setShowAuthorModal] = useState(false);
  const [authorModalMode, setAuthorModalMode] = useState("add");
  const [authorModalName, setAuthorModalName] = useState("");
  const [authorModalAvatar, setAuthorModalAvatar] = useState(null);
  const [authorModalCurrentAvatar, setAuthorModalCurrentAvatar] = useState(null);
  const [editingAuthorId, setEditingAuthorId] = useState(null);
  const [authorModalError, setAuthorModalError] = useState("");
  const [chords, setChords] = useState([]);
  const [showChordModal, setShowChordModal] = useState(false);
  const [chordModalMode, setChordModalMode] = useState("add"); // add | edit
  const [chordModalValue, setChordModalValue] = useState("");
  const [editingChordId, setEditingChordId] = useState(null);
  const [chordModalError, setChordModalError] = useState("");
  const [allChords, setAllChords] = useState([]);
  const [chordVariationsByChord, setChordVariationsByChord] = useState({});
  const [selectedVariationIdx, setSelectedVariationIdx] = useState({});
  const [chordEditorOpen, setChordEditorOpen] = useState(false);
  const [editingVariation, setEditingVariation] = useState(null);
  const [chordError, setChordError] = useState("");
  const [variationError, setVariationError] = useState("");
  const navigate = useNavigate();
  const { user, loading: authLoading } = useAuth();

  // Загрузка песен
  useEffect(() => {
    if (activeSection !== "songs") return;
    setLoading(true);
    fetch(`${API_BASE_URL}/api/songs`, { credentials: "include" })
      .then(res => res.json())
      .then(data => setSongs(data))
      .catch(() => setError("Ошибка загрузки песен"))
      .finally(() => setLoading(false));
  }, [activeSection]);

  // Загрузка подборок
  useEffect(() => {
    if (activeSection !== "collections") return;
    setLoading(true);
    fetch(`${API_BASE_URL}/api/collections`, { credentials: "include" })
      .then(res => res.json())
      .then(data => setCollections(data))
      .catch(() => setError("Ошибка загрузки подборок"))
      .finally(() => setLoading(false));
  }, [activeSection]);

  // Загрузка авторов
  useEffect(() => {
    if (activeSection !== "authors") return;
    setLoading(true);
    fetch(`${API_BASE_URL}/api/authors`, { credentials: "include" })
      .then(res => res.json())
      .then(data => setAuthors(data))
      .catch(() => setError("Ошибка загрузки авторов"))
      .finally(() => setLoading(false));
  }, [activeSection]);

  // Загрузка аккордов
  useEffect(() => {
    if (activeSection !== "chords") return;
    setLoading(true);
    fetch(`${API_BASE_URL}/api/chords`, { credentials: "include" })
      .then(res => res.json())
      .then(data => setChords(data))
      .catch(() => setError("Ошибка загрузки аккордов"))
      .finally(() => setLoading(false));
  }, [activeSection]);

  // Загрузка всех аккордов и вариаций для админки
  useEffect(() => {
    if (activeSection !== "chordVariations") return;
    setLoading(true);
    fetch(`${API_BASE_URL}/api/chords`, { credentials: "include" })
      .then(res => res.json())
      .then(async chords => {
        setAllChords(chords);
        // Для каждого аккорда загружаем вариации
        const variationsObj = {};
        await Promise.all(
          chords.map(async chord => {
            try {
              const res = await fetch(`${API_BASE_URL}/api/chord-variations/by-chord/${chord.id}`);
              if (res.ok) {
                const variations = await res.json();
                variationsObj[chord.name] = variations;
              } else {
                variationsObj[chord.name] = [];
              }
            } catch {
              variationsObj[chord.name] = [];
            }
          })
        );
        setChordVariationsByChord(variationsObj);
        // По умолчанию первая вариация
        const idxObj = {};
        chords.forEach(chord => {
          idxObj[chord.name] = 0;
        });
        setSelectedVariationIdx(idxObj);
      })
      .catch(() => setError("Ошибка загрузки аккордов/вариаций"))
      .finally(() => setLoading(false));
  }, [activeSection]);

  // Удаление песни
  const handleDeleteSong = async (id) => {
    if (authLoading) return;
    if (!user) {
      navigate("/login", { state: { from: window.location.pathname }, replace: true });
      return;
    }
    try {
      await fetchWithRefresh(`${API_BASE_URL}/api/users/me`);
    } catch (e) {
      if (e.message === "Failed to refresh token") {
        navigate("/login", { state: { from: window.location.pathname }, replace: true });
        return;
      }
    }
    if (!window.confirm("Удалить песню?")) return;
    setLoading(true);
    await fetch(`${API_BASE_URL}/api/songs/${id}`, {
      method: "DELETE",
      credentials: "include"
    });
    setSongs(songs => songs.filter(s => s.id !== id));
    setLoading(false);
  };

  // Удаление подборки
  const handleDeleteCollection = async (id) => {
    if (authLoading) return;
    if (!user) {
      navigate("/login", { state: { from: window.location.pathname }, replace: true });
      return;
    }
    try {
      await fetchWithRefresh(`${API_BASE_URL}/api/users/me`);
    } catch (e) {
      if (e.message === "Failed to refresh token") {
        navigate("/login", { state: { from: window.location.pathname }, replace: true });
        return;
      }
    }
    if (!window.confirm("Удалить подборку?")) return;
    setLoading(true);
    await fetch(`${API_BASE_URL}/api/collections/${id}`, {
      method: "DELETE",
      credentials: "include"
    });
    setCollections(collections => collections.filter(c => c.id !== id));
    setLoading(false);
  };

  // Редактирование песни
  const handleEditSong = (song) => {
    navigate("/add-song", { state: { song } });
  };

  // Добавление песни
  const handleAddSong = () => {
    navigate("/add-song");
  };

  // Открыть модалку для добавления подборки
  const handleAddCollection = () => {
    setCollectionModalMode("add");
    setCollectionModalValue("");
    setEditingCollectionId(null);
    setCollectionModalError("");
    setShowCollectionModal(true);
  };

  // Открыть модалку для редактирования подборки
  const handleEditCollection = (collection) => {
    setCollectionModalMode("edit");
    setCollectionModalValue(collection.name);
    setEditingCollectionId(collection.id);
    setCollectionModalError("");
    setShowCollectionModal(true);
  };

  // Сохранить подборку (добавить или обновить)
  const handleSaveCollection = async () => {
    if (authLoading) return;
    if (!user) {
      navigate("/login", { state: { from: window.location.pathname }, replace: true });
      return;
    }
    try {
      await fetchWithRefresh(`${API_BASE_URL}/api/users/me`);
    } catch (e) {
      if (e.message === "Failed to refresh token") {
        navigate("/login", { state: { from: window.location.pathname }, replace: true });
        return;
      }
    }
    if (!collectionModalValue.trim()) {
      setCollectionModalError("Название не должно быть пустым");
      return;
    }
    setLoading(true);
    setCollectionModalError("");
    if (collectionModalMode === "add") {
      const res = await fetch(`${API_BASE_URL}/api/collections`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({ name: collectionModalValue })
      });
      if (res.ok) {
        const newCol = await res.json();
        setCollections(cols => [...cols, newCol]);
        setShowCollectionModal(false);
      } else {
        setCollectionModalError("Ошибка при добавлении подборки");
      }
    } else if (collectionModalMode === "edit") {
      const res = await fetch(`${API_BASE_URL}/api/collections/${editingCollectionId}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({ name: collectionModalValue })
      });
      if (res.ok) {
        setCollections(cols => cols.map(c => c.id === editingCollectionId ? { ...c, name: collectionModalValue } : c));
        setShowCollectionModal(false);
      } else {
        setCollectionModalError("Ошибка при обновлении подборки");
      }
    }
    setLoading(false);
  };

  // Удаление автора
  const handleDeleteAuthor = async (id) => {
    if (authLoading) return;
    if (!user) {
      navigate("/login", { state: { from: window.location.pathname }, replace: true });
      return;
    }
    try {
      await fetchWithRefresh(`${API_BASE_URL}/api/users/me`);
    } catch (e) {
      if (e.message === "Failed to refresh token") {
        navigate("/login", { state: { from: window.location.pathname }, replace: true });
        return;
      }
    }
    if (!window.confirm("Удалить автора?")) return;
    setLoading(true);
    await fetch(`${API_BASE_URL}/api/authors/${id}`, {
      method: "DELETE",
      credentials: "include"
    });
    setAuthors(authors => authors.filter(a => a.id !== id));
    setLoading(false);
  };

  // Открыть модалку для добавления автора
  const handleAddAuthor = () => {
    setAuthorModalMode("add");
    setAuthorModalName("");
    setAuthorModalAvatar(null);
    setAuthorModalCurrentAvatar(null);
    setEditingAuthorId(null);
    setAuthorModalError("");
    setShowAuthorModal(true);
  };

  // Открыть модалку для редактирования автора
  const handleEditAuthor = (author) => {
    setAuthorModalMode("edit");
    setAuthorModalName(author.name);
    setAuthorModalAvatar(null);
    setAuthorModalCurrentAvatar(author.avatarPath ? API_BASE_URL + author.avatarPath : null);
    setEditingAuthorId(author.id);
    setAuthorModalError("");
    setShowAuthorModal(true);
  };

  // Удалить аватар у автора
  const handleDeleteAvatar = async () => {
    if (authLoading) return;
    if (!user) {
      navigate("/login", { state: { from: window.location.pathname }, replace: true });
      return;
    }
    try {
      await fetchWithRefresh(`${API_BASE_URL}/api/users/me`);
    } catch (e) {
      if (e.message === "Failed to refresh token") {
        navigate("/login", { state: { from: window.location.pathname }, replace: true });
        return;
      }
    }
    if (!editingAuthorId) return;
    setLoading(true);
    const res = await fetch(`${API_BASE_URL}/api/authors/${editingAuthorId}/avatar`, {
      method: "DELETE",
      credentials: "include"
    });
    if (res.ok) {
      setAuthors(auths => auths.map(a => a.id === editingAuthorId ? { ...a, avatarPath: null } : a));
      setAuthorModalCurrentAvatar(null);
    } else {
      setAuthorModalError("Ошибка при удалении аватара");
    }
    setLoading(false);
  };

  // Сохранить автора (добавить или обновить)
  const handleSaveAuthor = async () => {
    if (authLoading) return;
    if (!user) {
      navigate("/login", { state: { from: window.location.pathname }, replace: true });
      return;
    }
    try {
      await fetchWithRefresh(`${API_BASE_URL}/api/users/me`);
    } catch (e) {
      if (e.message === "Failed to refresh token") {
        navigate("/login", { state: { from: window.location.pathname }, replace: true });
        return;
      }
    }
    if (!authorModalName.trim()) {
      setAuthorModalError("Имя не должно быть пустым");
      return;
    }
    setLoading(true);
    setAuthorModalError("");
    const formData = new FormData();
    formData.append("Name", authorModalName);
    if (authorModalAvatar) formData.append("Avatar", authorModalAvatar);
    let ok = false;
    if (authorModalMode === "add") {
      const res = await fetch(`${API_BASE_URL}/api/authors`, {
        method: "POST",
        credentials: "include",
        body: formData
      });
      ok = res.ok;
    } else if (authorModalMode === "edit") {
      const res = await fetch(`${API_BASE_URL}/api/authors/${editingAuthorId}`, {
        method: "PUT",
        credentials: "include",
        body: formData
      });
      ok = res.ok;
    }
    if (ok) {
      // Обновляем список авторов с сервера
      try {
        const res = await fetch(`${API_BASE_URL}/api/authors`, { credentials: "include" });
        if (res.ok) {
          const data = await res.json();
          setAuthors(data);
        }
      } catch {}
      setShowAuthorModal(false);
    } else {
      setAuthorModalError("Ошибка при сохранении автора");
    }
    setLoading(false);
  };

  // Открыть модалку для добавления аккорда
  const handleAddChord = () => {
    setChordModalMode("add");
    setChordModalValue("");
    setEditingChordId(null);
    setChordModalError("");
    setShowChordModal(true);
  };

  // Открыть модалку для редактирования аккорда
  const handleEditChord = (chord) => {
    setChordModalMode("edit");
    setChordModalValue(chord.name);
    setEditingChordId(chord.id);
    setChordModalError("");
    setShowChordModal(true);
  };

  // Удаление аккорда
  const handleDeleteChord = async (id) => {
    if (authLoading) return;
    if (!user) {
      navigate("/login", { state: { from: window.location.pathname }, replace: true });
      return;
    }
    try {
      await fetchWithRefresh(`${API_BASE_URL}/api/users/me`);
    } catch (e) {
      if (e.message === "Failed to refresh token") {
        navigate("/login", { state: { from: window.location.pathname }, replace: true });
        return;
      }
    }
    if (!window.confirm("Удалить аккорд?")) return;
    setLoading(true);
    await fetch(`${API_BASE_URL}/api/chords/${id}`, {
      method: "DELETE",
      credentials: "include"
    });
    setChords(chords => chords.filter(c => c.id !== id));
    setLoading(false);
  };

  // Сохранить аккорд (добавить или обновить)
  const handleSaveChord = async () => {
    if (authLoading) return;
    if (!user) {
      navigate("/login", { state: { from: window.location.pathname }, replace: true });
      return;
    }
    try {
      await fetchWithRefresh(`${API_BASE_URL}/api/users/me`);
    } catch (e) {
      if (e.message === "Failed to refresh token") {
        navigate("/login", { state: { from: window.location.pathname }, replace: true });
        return;
      }
    }
    if (!chordModalValue.trim()) {
      setChordModalError("Название не должно быть пустым");
      return;
    }
    setLoading(true);
    setChordModalError("");
    let ok = false;
    if (chordModalMode === "add") {
      const res = await fetch(`${API_BASE_URL}/api/chords`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({ name: chordModalValue })
      });
      ok = res.ok;
    } else if (chordModalMode === "edit") {
      const res = await fetch(`${API_BASE_URL}/api/chords/${editingChordId}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({ name: chordModalValue })
      });
      ok = res.ok;
    }
    if (ok) {
      // Обновляем список аккордов с сервера
      try {
        const res = await fetch(`${API_BASE_URL}/api/chords`, { credentials: "include" });
        if (res.ok) {
          const data = await res.json();
          setChords(data);
        }
      } catch {}
      setShowChordModal(false);
    } else {
      setChordModalError("Ошибка при сохранении аккорда");
    }
    setLoading(false);
  };

  // Навигация по вариациям
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

  // Добавить вариацию
  const handleAddVariation = (chord) => {
    setEditingVariation({ chordName: chord.name, chordId: chord.id });
    setChordEditorOpen(true);
  };

  // Редактировать вариацию
  const handleEditVariation = (variation, chordName) => {
    setEditingVariation({ ...variation, chordName });
    setChordEditorOpen(true);
  };

  // Удалить вариацию
  const handleDeleteVariation = async (variationId, chordName) => {
    if (authLoading) return;
    if (!user) {
      navigate("/login", { state: { from: window.location.pathname }, replace: true });
      return;
    }
    try {
      await fetchWithRefresh(`${API_BASE_URL}/api/users/me`);
    } catch (e) {
      if (e.message === "Failed to refresh token") {
        navigate("/login", { state: { from: window.location.pathname }, replace: true });
        return;
      }
    }
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

  // Сохранить вариацию (добавить или обновить)
  const handleSaveVariation = async (data) => {
    if (authLoading) return;
    if (!user) {
      navigate("/login", { state: { from: window.location.pathname }, replace: true });
      return;
    }
    try {
      await fetchWithRefresh(`${API_BASE_URL}/api/users/me`);
    } catch (e) {
      if (e.message === "Failed to refresh token") {
        navigate("/login", { state: { from: window.location.pathname }, replace: true });
        return;
      }
    }
    setChordError("");
    setVariationError("");
    const { chordName, applicatura, startFret, bare, fingeringSVG } = data;
    let chordId = editingVariation?.chordId;
    let createdChord = null;
    if (!chordId) {
      // Найти аккорд по имени
      const chord = allChords.find(c => c.name === chordName);
      if (chord) chordId = chord.id;
    }
    if (!chordId) {
      // Создать аккорд, если не найден
      const res = await fetchWithRefresh(`${API_BASE_URL}/api/chords`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({ name: chordName })
      });
      if (res.ok) {
        createdChord = await res.json();
        chordId = createdChord.id;
        setAllChords(prev => [...prev, createdChord]);
      } else {
        setChordError("Ошибка при создании аккорда");
        return;
      }
    }
    // Добавление или обновление вариации
    let ok = false;
    if (!editingVariation?.id) {
      // Добавление
      const res = await fetchWithRefresh(`${API_BASE_URL}/api/chord-variations`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({ chordId, applicatura, startFret, bare, fingeringSVG })
      });
      ok = res.ok;
    } else {
      // Обновление
      const res = await fetchWithRefresh(`${API_BASE_URL}/api/chord-variations/${editingVariation.id}`, {
        method: "PUT",
        headers: { "Content-Type": "application/json" },
        credentials: "include",
        body: JSON.stringify({ chordId, applicatura, startFret, bare, fingeringSVG })
      });
      ok = res.ok;
    }
    if (ok) {
      // Обновляем вариации для этого аккорда
      try {
        const res = await fetch(`${API_BASE_URL}/api/chord-variations/by-chord/${chordId}`);
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
        }
      } catch {}
      setChordEditorOpen(false);
      setEditingVariation(null);
    } else {
      setVariationError("Ошибка при сохранении вариации");
    }
  };

  return (
    <div style={{ maxWidth: 900, margin: "40px auto", background: "#fff", borderRadius: 12, boxShadow: "0 2px 12px #0001", padding: 32 }}>
      <h2 style={{ marginBottom: 24 }}>Админ-панель</h2>
      <div style={{ display: "flex", gap: 16, marginBottom: 24 }}>
        {sections.map(sec => (
          <button
            key={sec.key}
            onClick={() => setActiveSection(sec.key)}
            style={{
              padding: "8px 18px",
              borderRadius: 8,
              border: activeSection === sec.key ? "2px solid #2980b9" : "1.5px solid #ccc",
              background: activeSection === sec.key ? "#e6f0fa" : "#f7faff",
              color: activeSection === sec.key ? "#2980b9" : "#222",
              fontWeight: 500,
              cursor: "pointer"
            }}
          >
            {sec.label}
          </button>
        ))}
      </div>
      <div>
        {activeSection === "songs" && (
          <div>
            <div style={{display: 'flex', alignItems: 'center', marginBottom: 16}}>
              <h3 style={{marginRight: 20}}>Список песен</h3>
              <button onClick={handleAddSong} style={{padding: '8px 18px', borderRadius: 8, background: '#2980b9', color: '#fff', border: 'none', fontWeight: 500, cursor: 'pointer'}}>Добавить</button>
            </div>
            {loading && <div>Загрузка...</div>}
            {error && <div style={{color: 'red'}}>{error}</div>}
            <table style={{width: '100%', borderCollapse: 'collapse'}}>
              <thead>
                <tr style={{background: '#f7faff'}}>
                  <th style={{padding: 8, border: '1px solid #eee'}}>ID</th>
                  <th style={{padding: 8, border: '1px solid #eee'}}>Название</th>
                  <th style={{padding: 8, border: '1px solid #eee'}}>Автор</th>
                  <th style={{padding: 8, border: '1px solid #eee'}}>Действия</th>
                </tr>
              </thead>
              <tbody>
                {songs.map(song => (
                  <tr key={song.id}>
                    <td style={{padding: 8, border: '1px solid #eee'}}>{song.id}</td>
                    <td style={{padding: 8, border: '1px solid #eee'}}>{song.name}</td>
                    <td style={{padding: 8, border: '1px solid #eee'}}>{song.userLogin}</td>
                    <td style={{padding: 8, border: '1px solid #eee'}}>
                      <button onClick={() => handleEditSong(song)} style={{marginRight: 8}}>Редактировать</button>
                      <button onClick={() => handleDeleteSong(song.id)} style={{color: 'red'}}>Удалить</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {songs.length === 0 && !loading && <div>Нет песен</div>}
          </div>
        )}
        {activeSection === "collections" && (
          <div>
            <div style={{display: 'flex', alignItems: 'center', marginBottom: 16}}>
              <h3 style={{marginRight: 20}}>Список подборок</h3>
              <button onClick={handleAddCollection} style={{padding: '8px 18px', borderRadius: 8, background: '#2980b9', color: '#fff', border: 'none', fontWeight: 500, cursor: 'pointer'}}>Добавить</button>
            </div>
            {loading && <div>Загрузка...</div>}
            {error && <div style={{color: 'red'}}>{error}</div>}
            <table style={{width: '100%', borderCollapse: 'collapse'}}>
              <thead>
                <tr style={{background: '#f7faff'}}>
                  <th style={{padding: 8, border: '1px solid #eee'}}>ID</th>
                  <th style={{padding: 8, border: '1px solid #eee'}}>Название</th>
                  <th style={{padding: 8, border: '1px solid #eee'}}>Действия</th>
                </tr>
              </thead>
              <tbody>
                {collections.map(col => (
                  <tr key={col.id}>
                    <td style={{padding: 8, border: '1px solid #eee'}}>{col.id}</td>
                    <td style={{padding: 8, border: '1px solid #eee'}}>{col.name}</td>
                    <td style={{padding: 8, border: '1px solid #eee'}}>
                      <button onClick={() => handleEditCollection(col)} style={{marginRight: 8}}>Редактировать</button>
                      <button onClick={() => handleDeleteCollection(col.id)} style={{color: 'red'}}>Удалить</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {collections.length === 0 && !loading && <div>Нет подборок</div>}
            {showCollectionModal && (
              <div style={{position: 'fixed', top: 0, left: 0, width: '100vw', height: '100vh', background: 'rgba(0,0,0,0.25)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000}}>
                <div style={{background: '#fff', borderRadius: 12, padding: 32, minWidth: 320, boxShadow: '0 2px 12px #0002', position: 'relative'}}>
                  <h4 style={{marginBottom: 18}}>{collectionModalMode === 'add' ? 'Добавить подборку' : 'Редактировать подборку'}</h4>
                  <input
                    type="text"
                    value={collectionModalValue}
                    onChange={e => setCollectionModalValue(e.target.value)}
                    placeholder="Название подборки"
                    style={{width: '100%', padding: 10, fontSize: 16, borderRadius: 6, border: '1.5px solid #ccc', marginBottom: 12}}
                    maxLength={30}
                  />
                  {collectionModalError && <div style={{color: 'red', marginBottom: 8}}>{collectionModalError}</div>}
                  <div style={{display: 'flex', gap: 12, justifyContent: 'flex-end'}}>
                    <button onClick={() => setShowCollectionModal(false)} style={{padding: '8px 18px', borderRadius: 8, background: '#eee', color: '#222', border: 'none', fontWeight: 500, cursor: 'pointer'}}>Отмена</button>
                    <button onClick={handleSaveCollection} style={{padding: '8px 18px', borderRadius: 8, background: '#2980b9', color: '#fff', border: 'none', fontWeight: 500, cursor: 'pointer'}}>Сохранить</button>
                  </div>
                </div>
              </div>
            )}
          </div>
        )}
        {activeSection === "authors" && (
          <div>
            <div style={{display: 'flex', alignItems: 'center', marginBottom: 16}}>
              <h3 style={{marginRight: 20}}>Список авторов</h3>
              <button onClick={handleAddAuthor} style={{padding: '8px 18px', borderRadius: 8, background: '#2980b9', color: '#fff', border: 'none', fontWeight: 500, cursor: 'pointer'}}>Добавить</button>
            </div>
            {loading && <div>Загрузка...</div>}
            {error && <div style={{color: 'red'}}>{error}</div>}
            <table style={{width: '100%', borderCollapse: 'collapse'}}>
              <thead>
                <tr style={{background: '#f7faff'}}>
                  <th style={{padding: 8, border: '1px solid #eee'}}>ID</th>
                  <th style={{padding: 8, border: '1px solid #eee'}}>Аватар</th>
                  <th style={{padding: 8, border: '1px solid #eee'}}>Имя</th>
                  <th style={{padding: 8, border: '1px solid #eee'}}>Действия</th>
                </tr>
              </thead>
              <tbody>
                {authors.map(author => (
                  <tr key={author.id}>
                    <td style={{padding: 8, border: '1px solid #eee'}}>{author.id}</td>
                    <td style={{padding: 8, border: '1px solid #eee'}}>
                      <img
                        src={author.avatarPath ? API_BASE_URL + author.avatarPath : "/img/placeholder.png"}
                        alt={author.name}
                        style={{width: 48, height: 48, borderRadius: '50%', objectFit: 'cover', border: '2px solid #5095db', background: '#f5f5f5'}}
                      />
                    </td>
                    <td style={{padding: 8, border: '1px solid #eee'}}>{author.name}</td>
                    <td style={{padding: 8, border: '1px solid #eee'}}>
                      <button onClick={() => handleEditAuthor(author)} style={{marginRight: 8}}>Редактировать</button>
                      <button onClick={() => handleDeleteAuthor(author.id)} style={{color: 'red'}}>Удалить</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {authors.length === 0 && !loading && <div>Нет авторов</div>}
            {showAuthorModal && (
              <div style={{position: 'fixed', top: 0, left: 0, width: '100vw', height: '100vh', background: 'rgba(0,0,0,0.25)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000}}>
                <div style={{background: '#fff', borderRadius: 12, padding: 32, minWidth: 320, boxShadow: '0 2px 12px #0002', position: 'relative'}}>
                  <h4 style={{marginBottom: 18}}>{authorModalMode === 'add' ? 'Добавить автора' : 'Редактировать автора'}</h4>
                  <input
                    type="text"
                    value={authorModalName}
                    onChange={e => setAuthorModalName(e.target.value)}
                    placeholder="Имя автора"
                    style={{width: '100%', padding: 10, fontSize: 16, borderRadius: 6, border: '1.5px solid #ccc', marginBottom: 12}}
                    maxLength={30}
                  />
                  {authorModalMode === 'edit' && authorModalCurrentAvatar && (
                    <div style={{marginBottom: 12, display: 'flex', alignItems: 'center', gap: 12}}>
                      <img src={authorModalCurrentAvatar} alt="avatar" style={{width: 48, height: 48, borderRadius: '50%', objectFit: 'cover', border: '2px solid #5095db', background: '#f5f5f5'}} />
                      <button onClick={handleDeleteAvatar} style={{padding: '6px 14px', borderRadius: 8, background: '#e74c3c', color: '#fff', border: 'none', fontWeight: 500, cursor: 'pointer'}}>Удалить аватар</button>
                    </div>
                  )}
                  <input
                    type="file"
                    accept="image/*"
                    onChange={e => setAuthorModalAvatar(e.target.files[0])}
                    style={{marginBottom: 12}}
                  />
                  {authorModalError && <div style={{color: 'red', marginBottom: 8}}>{authorModalError}</div>}
                  <div style={{display: 'flex', gap: 12, justifyContent: 'flex-end'}}>
                    <button onClick={() => setShowAuthorModal(false)} style={{padding: '8px 18px', borderRadius: 8, background: '#eee', color: '#222', border: 'none', fontWeight: 500, cursor: 'pointer'}}>Отмена</button>
                    <button onClick={handleSaveAuthor} style={{padding: '8px 18px', borderRadius: 8, background: '#2980b9', color: '#fff', border: 'none', fontWeight: 500, cursor: 'pointer'}}>Сохранить</button>
                  </div>
                </div>
              </div>
            )}
          </div>
        )}
        {activeSection === "chordVariations" && (
          <div>
            <h3 style={{marginBottom: 16}}>Вариации аккордов</h3>
            {loading && <div>Загрузка...</div>}
            {error && <div style={{color: 'red'}}>{error}</div>}
            {allChords.length === 0 && !loading && <div>Нет аккордов</div>}
            {allChords.map(chord => (
              <div key={chord.id} style={{marginBottom: 32, border: '1px solid #eee', borderRadius: 8, padding: 16}}>
                <div style={{display: 'flex', alignItems: 'center', marginBottom: 10}}>
                  <span style={{fontWeight: 500, fontSize: 18, marginRight: 16}}>{chord.name}</span>
                  <button onClick={() => handleAddVariation(chord)} style={{padding: '6px 14px', borderRadius: 8, background: '#2980b9', color: '#fff', border: 'none', fontWeight: 500, cursor: 'pointer'}}>Добавить вариацию</button>
                </div>
                {chordVariationsByChord[chord.name] && chordVariationsByChord[chord.name].length > 0 ? (
                  <ChordFingeringBlock
                    chord={chord}
                    variations={chordVariationsByChord[chord.name]}
                    idx={selectedVariationIdx[chord.name] || 0}
                    onPrev={() => handlePrevVariation(chord.name)}
                    onNext={() => handleNextVariation(chord.name)}
                    onEdit={handleEditVariation}
                    onDelete={handleDeleteVariation}
                    user={true}
                    currentUserId={1}
                  />
                ) : (
                  <div style={{color: '#aaa', fontSize: 13, marginBottom: 8}}>Нет вариаций</div>
                )}
              </div>
            ))}
            <ChordEditorModal
              open={chordEditorOpen}
              onClose={() => { setChordEditorOpen(false); setEditingVariation(null); }}
              onSave={handleSaveVariation}
              chordError={chordError}
              variationError={variationError}
              initialData={editingVariation}
            />
          </div>
        )}
        {activeSection === "chords" && (
          <div>
            <div style={{display: 'flex', alignItems: 'center', marginBottom: 16}}>
              <h3 style={{marginRight: 20}}>Список аккордов</h3>
              <button onClick={handleAddChord} style={{padding: '8px 18px', borderRadius: 8, background: '#2980b9', color: '#fff', border: 'none', fontWeight: 500, cursor: 'pointer'}}>Добавить</button>
            </div>
            {loading && <div>Загрузка...</div>}
            {error && <div style={{color: 'red'}}>{error}</div>}
            <table style={{width: '100%', borderCollapse: 'collapse'}}>
              <thead>
                <tr style={{background: '#f7faff'}}>
                  <th style={{padding: 8, border: '1px solid #eee'}}>ID</th>
                  <th style={{padding: 8, border: '1px solid #eee'}}>Название</th>
                  <th style={{padding: 8, border: '1px solid #eee'}}>Действия</th>
                </tr>
              </thead>
              <tbody>
                {chords.map(chord => (
                  <tr key={chord.id}>
                    <td style={{padding: 8, border: '1px solid #eee'}}>{chord.id}</td>
                    <td style={{padding: 8, border: '1px solid #eee'}}>{chord.name}</td>
                    <td style={{padding: 8, border: '1px solid #eee'}}>
                      <button onClick={() => handleEditChord(chord)} style={{marginRight: 8}}>Редактировать</button>
                      <button onClick={() => handleDeleteChord(chord.id)} style={{color: 'red'}}>Удалить</button>
                    </td>
                  </tr>
                ))}
              </tbody>
            </table>
            {chords.length === 0 && !loading && <div>Нет аккордов</div>}
            {showChordModal && (
              <div style={{position: 'fixed', top: 0, left: 0, width: '100vw', height: '100vh', background: 'rgba(0,0,0,0.25)', display: 'flex', alignItems: 'center', justifyContent: 'center', zIndex: 1000}}>
                <div style={{background: '#fff', borderRadius: 12, padding: 32, minWidth: 320, boxShadow: '0 2px 12px #0002', position: 'relative'}}>
                  <h4 style={{marginBottom: 18}}>{chordModalMode === 'add' ? 'Добавить аккорд' : 'Редактировать аккорд'}</h4>
                  <input
                    type="text"
                    value={chordModalValue}
                    onChange={e => setChordModalValue(e.target.value)}
                    placeholder="Название аккорда"
                    style={{width: '100%', padding: 10, fontSize: 16, borderRadius: 6, border: '1.5px solid #ccc', marginBottom: 12}}
                    maxLength={30}
                  />
                  {chordModalError && <div style={{color: 'red', marginBottom: 8}}>{chordModalError}</div>}
                  <div style={{display: 'flex', gap: 12, justifyContent: 'flex-end'}}>
                    <button onClick={() => setShowChordModal(false)} style={{padding: '8px 18px', borderRadius: 8, background: '#eee', color: '#222', border: 'none', fontWeight: 500, cursor: 'pointer'}}>Отмена</button>
                    <button onClick={handleSaveChord} style={{padding: '8px 18px', borderRadius: 8, background: '#2980b9', color: '#fff', border: 'none', fontWeight: 500, cursor: 'pointer'}}>Сохранить</button>
                  </div>
                </div>
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
} 