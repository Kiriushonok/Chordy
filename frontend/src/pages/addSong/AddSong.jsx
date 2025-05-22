import React, { useState, useEffect, useRef } from "react";
import "./AddSong.css";
import ChordEditorModal from "../../components/ChordEditingModal/ChordEditorModal";
import fetchWithRefresh from '../../utils/fetchWithRefresh';
import { useNavigate, useLocation } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';
import editIcon from '../../assets/img/edit.svg';
import xmarkIcon from '../../assets/img/xmark.svg';
import ChordFingeringBlock from "../../components/ChordFingeringBlock";

const API_BASE_URL = "https://localhost:7007";

const AddSong = () => {
    const [name, setName] = useState("");
    const [authorInput, setAuthorInput] = useState("");
    const [authorOptions, setAuthorOptions] = useState([]);
    const [authorDropdownOpen, setAuthorDropdownOpen] = useState(false);
    const [selectedAuthors, setSelectedAuthors] = useState([]);
    const [collectionIds, setCollectionIds] = useState([]);
    const [collections, setCollections] = useState([]);
    const [text, setText] = useState("");
    const [isPublic, setIsPublic] = useState(true);
    const [loading, setLoading] = useState(false);
    const [error, setError] = useState("");
    const [success, setSuccess] = useState(false);
    const [dropdownOpen, setDropdownOpen] = useState(false);
    const dropDownRef = useRef(null);
    const authorDropdownRef = useRef(null);
    const [nameError, setNameError] = useState("");
    const [authorError, setAuthorError] = useState("");
    const [addAuthorLoading, setAddAuthorLoading] = useState(false);
    const [chordEditorOpen, setChordEditorOpen] = useState(false);
    const [chordVariations, setChordVariations] = useState([]);
    const [allChords, setAllChords] = useState([]);
    const [highlightedChords, setHighlightedChords] = useState([]);
    const [chordVariationsByChord, setChordVariationsByChord] = useState({});
    const [selectedVariationIdx, setSelectedVariationIdx] = useState({});
    const [chordError, setChordError] = useState("");
    const [variationError, setVariationError] = useState("");
    const [fieldErrors, setFieldErrors] = useState({});
    const navigate = useNavigate();
    const location = useLocation();
    const { user, loading: authLoading } = useAuth();
    const [editingVariation, setEditingVariation] = useState(null);
    const textareaRef = useRef(null);
    const [editMode, setEditMode] = useState(false);
    const [editSongId, setEditSongId] = useState(null);

    useEffect(() => {
        fetch(`${API_BASE_URL}/api/collections`)
            .then((res) => res.json())
            .then(setCollections)
            .catch(() => setCollections([]));
    }, []);

    useEffect(() => {
        function handleClickOutside(event) {
            if (dropDownRef.current && !dropDownRef.current.contains(event.target)) {
                setDropdownOpen(false);
            }
        }
        if (dropdownOpen) {
            document.addEventListener("mousedown", handleClickOutside);
        } else {
            document.removeEventListener("mousedown", handleClickOutside);
        }
        return () => {
            document.removeEventListener("mousedown", handleClickOutside);
        };
    }, [dropdownOpen]);

    useEffect(() => {
        if (!authorInput) {
            setAuthorOptions([]);
            setAuthorDropdownOpen(false);
            return;
        }
        fetch(`${API_BASE_URL}/api/authors/search?query=${encodeURIComponent(authorInput)}`)
            .then(res => res.json())
            .then(data => {
                setAuthorOptions(data);
                setAuthorDropdownOpen(true);
            });
    }, [authorInput]);

    useEffect(() => {
        function handleClickOutside(event) {
            if (authorDropdownRef.current && !authorDropdownRef.current.contains(event.target)) {
                setAuthorDropdownOpen(false);
            }
        }
        if (authorDropdownOpen) {
            document.addEventListener("mousedown", handleClickOutside);
        } else {
            document.removeEventListener("mousedown", handleClickOutside);
        }
        return () => {
            document.removeEventListener("mousedown", handleClickOutside);
        };
    }, [authorDropdownOpen]);

    useEffect(() => {
        fetch(`${API_BASE_URL}/api/chords`)
            .then(res => res.json())
            .then(setAllChords)
            .catch(() => setAllChords([]));
    }, []);

    useEffect(() => {
        if (textareaRef.current) {
            textareaRef.current.style.height = 'auto';
            textareaRef.current.style.height = textareaRef.current.scrollHeight + 'px';
        }
    }, [text]);

    useEffect(() => {
        if (location.state && location.state.song) {
            const s = location.state.song;
            setName(s.name || "");
            setText(s.text || "");
            setIsPublic(typeof s.isPublic === 'boolean' ? s.isPublic : true);
            setSelectedAuthors(s.authors || []);
            setCollectionIds(s.collections ? s.collections.map(col => col.id.toString()) : []);
            setEditMode(true);
            setEditSongId(s.id);
        }
    }, [location.state]);

    const validateName = (value) => {
        if (!value || value.length < 1) return "Название не должно быть пустым";
        if (value.length > 100) return "Название не должно превышать 100 символов";
        return "";
    };
    const validateAuthor = (value) => {
        if (!value || value.length < 1) return "Имя автора не должно быть пустым";
        if (value.length > 30) return "Имя автора не должно превышать 30 символов";
        return "";
    };

    const handleAddAuthor = async () => {
        setAuthorError("");
        const err = validateAuthor(authorInput);
        if (err) {
            setAuthorError(err);
            return;
        }
        setAddAuthorLoading(true);
        try {
            const formData = new FormData();
            formData.append("Name", authorInput);
            const res = await fetchWithRefresh(`${API_BASE_URL}/api/authors`, {
                method: "POST",
                body: formData,
                credentials: "include"
            });
            if (!res.ok) {
                const data = await res.json().catch(() => ({}));
                setAuthorError(data?.message || "Ошибка при добавлении автора");
                setAddAuthorLoading(false);
                return;
            }
            const newAuthor = await res.json();
            setSelectedAuthors([...selectedAuthors, newAuthor]);
            setAuthorInput("");
            setAuthorDropdownOpen(false);
        } catch (e) {
            if (e.message === "Failed to refresh token") {
                navigate("/login", {
                    state: {
                        from: location.pathname,
                        action: "add-author",
                        draft: { authorInput }
                    },
                    replace: true
                });
            } else {
                setAuthorError("Ошибка при добавлении автора");
            }
        } finally {
            setAddAuthorLoading(false);
        }
    };

    // Вспомогательная функция для выделения аккордов и получения их вариаций
    async function getChordsAndVariations(text, allChords) {
        if (!text) return { chordsArr: [], variationsObj: {}, selectedIdxObj: {} };
        const chordNames = allChords.map(c => c.name);
        const lines = text.split(/\r?\n/);
        const foundChords = new Set();
        lines.forEach(line => {
            const trimmed = line.trim();
            if (!trimmed) return;
            const words = trimmed.split(/\s+/);
            if (words.length > 0 && words.every(w => chordNames.includes(w))) {
                words.forEach(w => foundChords.add(w));
            }
        });
        const chordsArr = Array.from(foundChords);
        // Загружаем вариации для каждого аккорда
        const variationsObj = {};
        const selectedIdxObj = {};
        await Promise.all(
            chordsArr.map(async chordName => {
                const chord = allChords.find(c => c.name === chordName);
                if (!chord) return;
                try {
                    const res = await fetch(`${API_BASE_URL}/api/chord-variations/by-chord/${chord.id}`);
                    if (res.ok) {
                        const variations = await res.json();
                        variationsObj[chordName] = variations;
                        selectedIdxObj[chordName] = 0;
                    } else {
                        variationsObj[chordName] = [];
                        selectedIdxObj[chordName] = 0;
                    }
                } catch {
                    variationsObj[chordName] = [];
                    selectedIdxObj[chordName] = 0;
                }
            })
        );
        return { chordsArr, variationsObj, selectedIdxObj };
    }

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");
        setFieldErrors({});
        setSuccess(false);
        const nameErr = validateName(name);
        if (nameErr) {
            setNameError(nameErr);
            return;
        }
        // --- Новый блок: выделяем аккорды и вариации прямо перед отправкой ---
        const { chordsArr, variationsObj, selectedIdxObj } = await getChordsAndVariations(text, allChords);
        let defaultChordVariationIds = [];
        chordsArr.forEach(chord => {
            const variations = variationsObj[chord] || [];
            const idx = selectedIdxObj[chord] || 0;
            if (variations[idx]) {
                defaultChordVariationIds.push(variations[idx].id);
            }
        });
        // --- Обновляем стейты для UI (опционально, чтобы интерфейс тоже обновился) ---
        setHighlightedChords(chordsArr);
        setChordVariationsByChord(variationsObj);
        setSelectedVariationIdx(selectedIdxObj);
        setLoading(true);
        try {
            const body = {
                name,
                text,
                isPublic,
                authorIds: selectedAuthors.map(a => a.id),
                collectionIds: collectionIds.map(Number),
                defaultChordVariationIds
            };
            let res;
            if (editMode && editSongId) {
                res = await fetchWithRefresh(`${API_BASE_URL}/api/songs/${editSongId}`, {
                    method: "PUT",
                    headers: { "Content-Type": "application/json" },
                    credentials: "include",
                    body: JSON.stringify(body),
                });
                if (!res.ok) {
                    const err = await res.json().catch(() => ({}));
                    if (err && err.errors) {
                        setError(err.detail || "Недопустимое значение поля");
                        setFieldErrors(err.errors);
                    } else {
                        setError(err?.detail || err?.title || err?.message || "Ошибка при добавлении песни");
                    }
                    setLoading(false);
                    return;
                }
                navigate(`/songs/${editSongId}`);
                setSuccess(true);
                setName("");
                setSelectedAuthors([]);
                setCollectionIds([]);
                setText("");
                setIsPublic(true);
                setHighlightedChords([]);
                setChordVariationsByChord({});
                setSelectedVariationIdx({});
                return;
            } else {
                res = await fetchWithRefresh(`${API_BASE_URL}/api/songs`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    credentials: "include",
                    body: JSON.stringify(body),
                });
            }
            if (!res.ok) {
                const err = await res.json().catch(() => ({}));
                if (err && err.errors) {
                    setError(err.detail || "Недопустимое значение поля");
                    setFieldErrors(err.errors);
                } else {
                    setError(err?.detail || err?.title || err?.message || "Ошибка при добавлении песни");
                }
                setLoading(false);
                return;
            }
            const song = await res.json();
            navigate(`/songs/${song.id}`);
            setSuccess(true);
            setName("");
            setSelectedAuthors([]);
            setCollectionIds([]);
            setText("");
            setIsPublic(true);
            setHighlightedChords([]);
            setChordVariationsByChord({});
            setSelectedVariationIdx({});
        } catch (e) {
            if (e.message === "Failed to refresh token") {
                navigate("/login", {
                    state: {
                        from: location.pathname,
                        action: "add-song",
                        draft: {
                            name, text, isPublic, selectedAuthors, collectionIds, highlightedChords, chordVariationsByChord, selectedVariationIdx
                        }
                    },
                    replace: true
                });
            } else {
                setError(e.message);
            }
        } finally {
            setLoading(false);
        }
    };

    const handleAddChord = async () => {
        if (authLoading) return;
        if (!user) {
            navigate("/login", { state: { from: location.pathname, action: "add-chord" }, replace: true });
            return;
        }
        try {
            await fetchWithRefresh(`${API_BASE_URL}/api/users/me`);
            setChordEditorOpen(true);
        } catch (e) {
            if (e.message === "Failed to refresh token") {
                navigate("/login", { state: { from: location.pathname, action: "add-chord" }, replace: true });
            }
        }
    };

    const handleChordSave = async (data) => {
        setChordError("");
        setVariationError("");
        const { chordName, applicatura, startFret, bare } = data;
        if (!chordName || chordName.trim().length === 0) {
            setChordError("Имя аккорда обязательно");
            return;
        }
        let chordId = null;
        let createdChord = null;
        try {
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
                // Добавляем новый аккорд в allChords
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
                        draft: { chordName, applicatura, startFret, bare }
                    },
                    replace: true
                });
            } else {
                setChordError("Ошибка при поиске/создании аккорда");
            }
            return;
        }
        // 3. Сохраняем вариацию аккорда в БД
        const variationPayload = {
            chordId,
            applicatura,
            startFret,
            bare,
            fingeringSVG: data.fingeringSVG,
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
            const variation = await res.json();
            setChordVariations((prev) => [
                ...prev,
                variation
            ]);
            // Всегда подгружаем актуальные вариации для этого аккорда
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
                    }
                } catch {}
            }
            setChordEditorOpen(false);
        } catch (e) {
            if (e.message === "Failed to refresh token") {
                navigate("/login", {
                    state: {
                        from: location.pathname,
                        action: "add-chord-variation",
                        draft: { chordId, applicatura, startFret, bare, fingeringSVG: data.fingeringSVG }
                    },
                    replace: true
                });
            } else {
                setVariationError("Ошибка при сохранении вариации аккорда");
            }
            return;
        }
    };

    const handleHighlightChords = async () => {
        if (!text) return;
        const chordNames = allChords.map(c => c.name);
        const lines = text.split(/\r?\n/);
        const foundChords = new Set();
        lines.forEach(line => {
            const trimmed = line.trim();
            if (!trimmed) return;
            const words = trimmed.split(/\s+/);
            if (words.length > 0 && words.every(w => chordNames.includes(w))) {
                words.forEach(w => foundChords.add(w));
            }
        });
        const chordsArr = Array.from(foundChords);
        setHighlightedChords(chordsArr);
        // Загружаем вариации для каждого аккорда
        const variationsObj = {};
        const selectedIdxObj = {};
        await Promise.all(
            chordsArr.map(async chordName => {
                const chord = allChords.find(c => c.name === chordName);
                if (!chord) return;
                try {
                    const res = await fetch(`${API_BASE_URL}/api/chord-variations/by-chord/${chord.id}`);
                    if (res.ok) {
                        const variations = await res.json();
                        variationsObj[chordName] = variations;
                        selectedIdxObj[chordName] = 0; // по умолчанию первая вариация
                    } else {
                        variationsObj[chordName] = [];
                        selectedIdxObj[chordName] = 0;
                    }
                } catch {
                    variationsObj[chordName] = [];
                    selectedIdxObj[chordName] = 0;
                }
            })
        );
        setChordVariationsByChord(variationsObj);
        setSelectedVariationIdx(selectedIdxObj);
    };

    const handlePrevVariation = (chord) => {
        setSelectedVariationIdx(prev => {
            const max = (chordVariationsByChord[chord] || []).length;
            if (!max) return prev;
            return {
                ...prev,
                [chord]: (prev[chord] - 1 + max) % max
            };
        });
    };
    const handleNextVariation = (chord) => {
        setSelectedVariationIdx(prev => {
            const max = (chordVariationsByChord[chord] || []).length;
            if (!max) return prev;
            return {
                ...prev,
                [chord]: (prev[chord] + 1) % max
            };
        });
    };

    // Удаление вариации
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

    // Открыть модалку для редактирования
    const handleEditVariation = async (variation, chordName) => {
        if (authLoading) return;
        if (!user) {
            navigate("/login", { state: { from: location.pathname, action: "edit-chord" }, replace: true });
            return;
        }
        try {
            await fetchWithRefresh(`${API_BASE_URL}/api/users/me`);
            setEditingVariation({ ...variation, chordName });
            setChordEditorOpen(true);
        } catch (e) {
            if (e.message === "Failed to refresh token") {
                navigate("/login", { state: { from: location.pathname, action: "edit-chord" }, replace: true });
            }
        }
    };

    // Обновление вариации
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
            let updated = null;
            try {
                updated = await res.json();
            } catch {
                updated = null;
            }
            // После успешного обновления — обновляем вариации для этого аккорда с сервера
            if (editingVariation.chordName) {
                const chord = allChords.find(c => c.name === editingVariation.chordName);
                if (chord) {
                    try {
                        const res = await fetch(`${API_BASE_URL}/api/chord-variations/by-chord/${chord.id}`);
                        if (res.ok) {
                            const variations = await res.json();
                            setChordVariationsByChord(prev => ({
                                ...prev,
                                [editingVariation.chordName]: variations
                            }));
                        }
                    } catch {}
                }
            }
            setEditingVariation(null);
            setChordEditorOpen(false);
            setVariationError("");
        } catch (e) {
            setVariationError('Ошибка при обновлении вариации аккорда');
        }
    };

    return (
        
        <div className="add-song-main">
            <form className="add-song-form" onSubmit={handleSubmit}>
                <div className="add-song-fields">

                    <input
                        className="add-song-input"
                        type="text"
                        placeholder="Название песни"
                        value={name}
                        onChange={e => {
                            setName(e.target.value);
                            setNameError(validateName(e.target.value));
                        }}
                        onBlur={e => setNameError(validateName(e.target.value))}
                        required
                    />
                    {nameError && <div className="add-song-error">{nameError}</div>}
                    <div className="add-song-author-autocomplete" ref={authorDropdownRef}>
                        <input
                            className="add-song-input"
                            type="text"
                            placeholder="Введите автора..."
                            value={authorInput}
                            onChange={e => {
                                setAuthorInput(e.target.value);
                                setAuthorError("");
                            }}
                            onFocus={() => authorInput && setAuthorDropdownOpen(true)}
                            autoComplete="off"
                        />
                        {authorError && <div className="add-song-error">{authorError}</div>}
                        {authorDropdownOpen && (
                            <ul className="add-song-author-dropdown">
                                {authorOptions
                                    .filter(option => !selectedAuthors.some(a => a.id === option.id))
                                    .map(option => (
                                        <li
                                            key={option.id}
                                            className="add-song-author-dropdown-item"
                                            onClick={() => {
                                                if (!selectedAuthors.some(a => a.id === option.id)) {
                                                    setSelectedAuthors([...selectedAuthors, option]);
                                                }
                                                setAuthorInput("");
                                                setAuthorDropdownOpen(false);
                                            }}
                                        >
                                            {option.name}
                                        </li>
                                    ))}
                                {authorInput.trim() &&
                                    !authorOptions.some(
                                        option => option.name.trim().toLowerCase() === authorInput.trim().toLowerCase()
                                    ) && (
                                        <li
                                            className="add-song-author-dropdown-item add-song-author-add-btn"
                                            onClick={handleAddAuthor}
                                        >
                                            {addAuthorLoading ? "Добавление..." : `Добавить "${authorInput}"`}
                                        </li>
                                    )}
                            </ul>
                        )}
                        {selectedAuthors.length > 0 && (
                            <div className="add-song-author-tags">
                                {selectedAuthors.map(author => (
                                    <span className="add-song-author-tag" key={author.id || author.name}>
                                        {author.name}
                                        <button type="button" className="add-song-author-tag-remove" onClick={() => setSelectedAuthors(selectedAuthors.filter(a => (a.id || a.name) !== (author.id || author.name)))}>
                                            ×
                                        </button>
                                    </span>
                                ))}
                            </div>
                        )}
                    </div>
                    <div className="add-song-dropdown" ref={dropDownRef}>
                        <div
                            className={`add-song-dropdown-control${dropdownOpen ? ' open' : ''}`}
                            onClick={() => setDropdownOpen(open => !open)}
                        >
                            {collectionIds.length === 0 ? (
                                <span className="add-song-dropdown-placeholder">Выберите подборки</span>
                            ) : (
                                <span className="add-song-dropdown-values">
                                    {collections.filter(col => collectionIds.includes(col.id.toString())).map(col => col.name).join(', ')}
                                </span>
                            )}
                            <span className="add-song-dropdown-arrow">&#9660;</span>
                        </div>
                        {dropdownOpen && (
                            <ul className="add-song-dropdown-list">
                                {collections.map(col => (
                                    <li key={col.id} className="add-song-dropdown-item">
                                        <label>
                                            <input
                                                type="checkbox"
                                                checked={collectionIds.includes(col.id.toString())}
                                                onChange={e => {
                                                    if (e.target.checked) {
                                                        setCollectionIds([...collectionIds, col.id.toString()]);
                                                    } else {
                                                        setCollectionIds(collectionIds.filter(id => id !== col.id.toString()));
                                                    }
                                                }}
                                            />
                                            <span>{col.name}</span>
                                        </label>
                                    </li>
                                ))}
                            </ul>
                        )}
                    </div>
                    <textarea
                        className="add-song-textarea"
                        placeholder="Текст песни, аккорды, табулатуры..."
                        value={text}
                        onChange={e => setText(e.target.value)}
                        rows={12}
                        required
                        ref={textareaRef}
                    />
                </div>
                <div className="add-song-side">
                    <div className="add-song-controls">
                        <div className="add-song-chords-block">
                            {/* Блок для аппликатур аккордов */}
                            {highlightedChords.length > 0 && (
                                <div className="add-song-chords-list">
                                    {highlightedChords.map((chordName) => {
                                        const chordObj = allChords.find(c => c.name === chordName);
                                        if (!chordObj) return null;
                                        const variations = chordVariationsByChord[chordName] || [];
                                        const idx = selectedVariationIdx[chordName] || 0;
                                        return (
                                            <ChordFingeringBlock
                                                key={chordObj.id}
                                                chord={chordObj}
                                                variations={variations}
                                                idx={idx}
                                                onPrev={() => handlePrevVariation(chordName)}
                                                onNext={() => handleNextVariation(chordName)}
                                                onEdit={(variation, chordName) => handleEditVariation(variation, chordName)}
                                                onDelete={handleDeleteVariation}
                                                user={user}
                                                currentUserId={user?.id}
                                            />
                                        );
                                    })}
                                </div>
                            )}
                        </div>
                        <button
                            type="button"
                            className="add-song-btn"
                            onClick={handleAddChord}
                        >
                            Добавить аккорд
                        </button>
                        <button
                            type="button"
                            className="add-song-btn"
                            onClick={handleHighlightChords}
                        >
                            Выделить аккорды
                        </button>
                        <button className="add-song-btn" type="submit" disabled={loading}>
                            {loading ? "Сохранение..." : "Сохранить"}
                        </button>
                        <label className="add-song-switch">
                            <input
                                type="checkbox"
                                checked={isPublic}
                                onChange={() => setIsPublic(v => !v)}
                            />
                            <span className="add-song-switch-slider"></span>
                            <span className="add-song-switch-label">Видимость: {isPublic ? "Публичная" : "Приватная"}</span>
                        </label>
                        {error && <div className="add-song-error">{error}</div>}
                        {Object.keys(fieldErrors).length > 0 && (
                          <div className="add-song-error-list">
                            <ul>
                              {Object.entries(fieldErrors).map(([field, messages]) =>
                                messages.map((msg, idx) => (
                                  <li key={field + idx}>{msg}</li>
                                ))
                              )}
                            </ul>
                          </div>
                        )}
                        {success && <div className="add-song-success">Песня успешно добавлена!</div>}
                    </div>
                </div>
            </form>
            <ChordEditorModal
                open={chordEditorOpen}
                onClose={() => { setChordEditorOpen(false); setEditingVariation(null); }}
                onSave={editingVariation ? handleUpdateVariation : handleChordSave}
                chordError={chordError}
                variationError={variationError}
                initialData={editingVariation}
            />
        </div>
    );
};

export default AddSong;
