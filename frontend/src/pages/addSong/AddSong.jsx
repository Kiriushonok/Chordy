import React, { useState, useEffect, useRef } from "react";
import "./AddSong.css";
import ChordEditorModal from "../../components/ChordEditingModal/ChordEditorModal";

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
            const res = await fetch(`${API_BASE_URL}/api/authors`, {
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
            setAuthorError("Ошибка при добавлении автора");
        } finally {
            setAddAuthorLoading(false);
        }
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setError("");
        setSuccess(false);
        const nameErr = validateName(name);
        if (nameErr) {
            setNameError(nameErr);
            return;
        }
        setLoading(true);
        try {
            // Собираем id выбранных вариаций
            let defaultChordVariationIds = [];
            highlightedChords.forEach(chord => {
                const variations = chordVariationsByChord[chord] || [];
                const idx = selectedVariationIdx[chord] || 0;
                if (variations[idx]) {
                    defaultChordVariationIds.push(variations[idx].id);
                }
            });
            const res = await fetch(`${API_BASE_URL}/api/songs`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify({
                    name,
                    text,
                    isPublic,
                    authorIds: selectedAuthors.map(a => a.id),
                    collectionIds: collectionIds.map(Number),
                    defaultChordVariationIds
                }),
            });
            if (!res.ok) throw new Error("Ошибка при добавлении песни");
            setSuccess(true);
            setName("");
            setSelectedAuthors([]);
            setCollectionIds([]);
            setText("");
            setIsPublic(true);
        } catch (e) {
            setError(e.message);
        } finally {
            setLoading(false);
        }
    };

    const handleAddChord = () => {
        setChordEditorOpen(true);
    };

    const handleChordSave = async (data) => {
        const { chordName, applicatura, startFret, bare } = data;
        if (!chordName || chordName.trim().length === 0) {
            alert("Имя аккорда обязательно");
            return;
        }
        let chordId = null;
        try {
            // 1. Пробуем найти аккорд по имени
            const res = await fetch(`${API_BASE_URL}/api/chords/by-name/${encodeURIComponent(chordName)}`);
            if (res.ok) {
                const chord = await res.json();
                chordId = chord.id;
            } else if (res.status === 404) {
                // 2. Если не найден — создаём
                const createRes = await fetch(`${API_BASE_URL}/api/chords`, {
                    method: "POST",
                    headers: { "Content-Type": "application/json" },
                    credentials: "include",
                    body: JSON.stringify({ name: chordName })
                });
                if (!createRes.ok) {
                    const err = await createRes.json().catch(() => ({}));
                    alert(err?.errors?.Name?.join(". ") || "Ошибка при создании аккорда");
                    return;
                }
                const newChord = await createRes.json();
                chordId = newChord.id;
            } else {
                alert("Ошибка при поиске аккорда");
                return;
            }
        } catch (e) {
            alert("Ошибка при поиске/создании аккорда");
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
        console.log('Вариация аккорда для отправки:', variationPayload);
        try {
            const res = await fetch(`${API_BASE_URL}/api/chord-variations`, {
                method: "POST",
                headers: { "Content-Type": "application/json" },
                credentials: "include",
                body: JSON.stringify(variationPayload),
            });
            if (!res.ok) {
                const err = await res.json().catch(() => ({}));
                alert(err?.message || "Ошибка при сохранении вариации аккорда");
                return;
            }
            const variation = await res.json();
            setChordVariations((prev) => [
                ...prev,
                variation
            ]);
        } catch (e) {
            alert("Ошибка при сохранении вариации аккорда");
            return;
        }
        setChordEditorOpen(false);
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
                    />
                </div>
                <div className="add-song-side">
                    <div className="add-song-controls">
                        <div className="add-song-chords-block">
                            {/* Блок для аппликатур аккордов */}
                            {highlightedChords.length > 0 && (
                                <div style={{ marginBottom: 12 }}>
                                    <b>Найденные аккорды:</b>
                                    <ul style={{ margin: 0, padding: 0, listStyle: 'none' }}>
                                        {highlightedChords.map(chord => {
                                            const variations = chordVariationsByChord[chord] || [];
                                            const idx = selectedVariationIdx[chord] || 0;
                                            const current = variations[idx];
                                            return (
                                                <li key={chord} style={{ marginBottom: 24 }}>
                                                    <div style={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
                                                        {current ? (
                                                            <>
                                                                <div style={{ background: '#fff', border: '1px solid #b3e672', borderRadius: 8, padding: 6, marginBottom: 8 }}>
                                                                    <div dangerouslySetInnerHTML={{ __html: current.fingeringSVG }} />
                                                                </div>
                                                                <div style={{ display: 'flex', justifyContent: 'center', gap: 16, marginBottom: 6 }}>
                                                                    <button type="button" className="add-song-btn" style={{ minWidth: 36, padding: '2px 8px' }} onClick={() => handlePrevVariation(chord)}>&larr;</button>
                                                                    <span style={{ fontWeight: 500, minWidth: 40, textAlign: 'center' }}>{idx + 1} / {variations.length}</span>
                                                                    <button type="button" className="add-song-btn" style={{ minWidth: 36, padding: '2px 8px' }} onClick={() => handleNextVariation(chord)}>&rarr;</button>
                                                                </div>
                                                                <div style={{ fontWeight: 600, fontSize: 16 }}>{chord}</div>
                                                            </>
                                                        ) : (
                                                            <span style={{ color: '#aaa', fontSize: 13 }}>Нет вариаций</span>
                                                        )}
                                                    </div>
                                                </li>
                                            );
                                        })}
                                    </ul>
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
                        {success && <div className="add-song-success">Песня успешно добавлена!</div>}
                    </div>
                </div>
            </form>
            <ChordEditorModal
                open={chordEditorOpen}
                onClose={() => setChordEditorOpen(false)}
                onSave={handleChordSave}
            />
        </div>
    );
};

export default AddSong;
