import React, { useState, useEffect, useRef } from "react";
import "./ChordEditorModal.css";

const STRINGS_COUNT = 6;
const FRETS_COUNT = 5;
const FINGER_COLORS = ["#6ee16e", "#6ee1c7", "#6eaee1", "#e1e16e"];

function getDefaultApplicatura(startFret) {
  return {
    strings: Array.from({ length: STRINGS_COUNT }, () => ({
      state: "open",
      presses: [],
    })),
    startFret,
  };
}

function findBarre(applicatura) {
  // Возвращает {fret, finger, strings: [индексы струн]} если есть баррэ, иначе null
  const map = {};
  applicatura.strings.forEach((str, sIdx) => {
    str.presses.forEach(press => {
      const key = `${press.fret}:${press.finger}`;
      if (!map[key]) map[key] = [];
      map[key].push(sIdx);
    });
  });
  for (const key in map) {
    if (map[key].length >= 2) {
      const [fret, finger] = key.split(":").map(Number);
      return { fret, finger, strings: map[key] };
    }
  }
  return null;
}

function generateChordSVG({ applicatura, startFret }) {
  const width = 100;
  const height = 100;
  const fretCount = 5;
  const stringCount = 6;
  const fretSpacing = 18;
  const stringSpacing = 14;
  const nutHeight = 3;
  const marginLeft = 18;
  const marginTop = 10;
  const circleRadius = 6.5;
  const fontSize = 12;
  const lableFontSize = 14;
  // Определяем баррэ
  const barre = findBarre(applicatura);
  // SVG строки
  let svg = `<svg width="${width}" height="${height}" viewBox="0 0 ${width} ${height}" xmlns="http://www.w3.org/2000/svg">`;
  // Верхняя линия (верх грифа)
  if (startFret === 1) {
    svg += `<rect x="${marginLeft - 3}" y="${marginTop - nutHeight / 2}" width="${stringSpacing * (stringCount - 1) + 6}" height="${nutHeight}" rx="2" fill="#111"/>`;
  }
  // Вертикальные линии (струны)
  for (let i = 0; i < stringCount; i++) {
    svg += `<line x1="${marginLeft + i * stringSpacing}" y1="${marginTop}" x2="${marginLeft + i * stringSpacing}" y2="${marginTop + fretSpacing * fretCount}" stroke="#2d2d2d" stroke-width="1"/>`;
  }
  // Горизонтальные линии (лады)
  for (let i = 0; i <= fretCount; i++) {
    svg += `<line x1="${marginLeft}" y1="${marginTop + i * fretSpacing}" x2="${marginLeft + stringSpacing * (stringCount - 1)}" y2="${marginTop + i * fretSpacing}" stroke="#2d2d2d" stroke-width="1"/>`;
  }
  // Номера ладов слева
  for (let i = 1; i <= fretCount; i++) {
    svg += `<text x="${marginLeft - 10}" y="${marginTop + i * fretSpacing - fretSpacing / 2 + 4}" text-anchor="middle" font-size="${lableFontSize}" fill="#111">${startFret + i - 1}</text>`;
  }
  // Баррэ
  if (barre) {
    const fretIdx = barre.fret - startFret + 1;
    if (fretIdx >= 1 && fretIdx <= fretCount) {
      // Центр лада: середина между двумя ладами
      const y = marginTop + (fretIdx - 0.5) * fretSpacing + (circleRadius * 2 - 10)/2;
      // Группируем струны по последовательным участкам
      const sorted = [...barre.strings].sort((a, b) => a - b);
      let group = [sorted[0]];
      for (let i = 1; i <= sorted.length; i++) {
        if (i < sorted.length && sorted[i] === sorted[i - 1] + 1) {
          group.push(sorted[i]);
        } else {
          // Нарисовать баррэ для текущей группы
          const x1 = marginLeft + group[0] * stringSpacing;
          const x2 = marginLeft + group[group.length - 1] * stringSpacing;
          const barreWidth = (x2 - x1 + 2 * circleRadius) * 0.95;
          const barreX = x1 + ((x2 - x1 + 2 * circleRadius) - barreWidth) / 2 - circleRadius;
          svg += `<rect x="${barreX}" y="${y - circleRadius + 3}" width="${barreWidth}" height="${circleRadius * 2 - 10}" rx="${circleRadius * 0.7}" fill="#111" stroke="#111" stroke-width="1"/>`;
          // Начать новую группу, если есть ещё струны
          if (i < sorted.length) group = [sorted[i]];
        }
      }
    }
  }
  // Обычные прижатия (без смещения)
  applicatura.strings.forEach((str, sIdx) => {
    str.presses.forEach(press => {
      // Не рисуем кружок, если это баррэ
      if (barre && press.fret === barre.fret && press.finger === barre.finger && barre.strings.includes(sIdx)) return;
      const fretIdx = press.fret - startFret + 1;
      if (fretIdx >= 1 && fretIdx <= fretCount) {
        const x = marginLeft + sIdx * stringSpacing;
        const y = marginTop + fretIdx * fretSpacing - fretSpacing / 2;
        svg += `<circle cx="${x}" cy="${y}" r="${circleRadius}" fill="#111" stroke="#111" stroke-width="1" />`;
        svg += `<text x="${x}" y="${y+1}" text-anchor="middle" alignment-baseline="middle" font-size="${fontSize}" fill="#fff" font-family="Arial, Arial Black, sans-serif">${press.finger}</text>`;
      }
    });
  });
  // Метки "X" и "O" сверху
  applicatura.strings.forEach((str, sIdx) => {
    const x = marginLeft + sIdx * stringSpacing;
    const y = marginTop - 5;
    if (str.state === "muted") {
      svg += `<text x="${x}" y="${y}" text-anchor="middle" font-size="${fontSize + 2}" fill="#111" font-weight="bold">X</text>`;
    } else if (str.state === "open") {
      svg += `<text x="${x}" y="${y}" text-anchor="middle" font-size="${fontSize + 2}" fill="#111" font-weight="bold">O</text>`;
    }
  });
  svg += `</svg>`;
  return svg;
}

export default function ChordEditorModal({ open, onClose, onSave, chordError, variationError, initialData }) {
  if (!open) return null;
  const [startFret, setStartFret] = useState(1);
  const [applicatura, setApplicatura] = useState(getDefaultApplicatura(1));
  const [editing, setEditing] = useState(null); // { stringIdx, fretIdx }
  const [editingFret, setEditingFret] = useState(false); // true если редактируем начальный лад
  const [fretInputValue, setFretInputValue] = useState("1");
  const fretInputRef = useRef(null);
  const [editingFinger, setEditingFinger] = useState(null); // { stringIdx, fretIdx }
  const [fingerInputValue, setFingerInputValue] = useState("");
  const fingerInputRef = useRef(null);
  const modalRef = useRef(null);
  const [chordName, setChordName] = useState("");

  useEffect(() => {
    if (open) {
      if (initialData) {
        setChordName(initialData.chordName || "");
        setStartFret(initialData.startFret || 1);
        setApplicatura(initialData.applicatura || getDefaultApplicatura(initialData.startFret || 1));
        setFretInputValue(initialData.startFret ? String(initialData.startFret) : "1");
      } else {
        setChordName("");
        setStartFret(1);
        setApplicatura(getDefaultApplicatura(1));
        setFretInputValue("1");
      }
      setEditing(null);
      setEditingFret(false);
      setEditingFinger(null);
      setFingerInputValue("");
    }
  }, [open, initialData]);

  // Фокус на input начального лада
  useEffect(() => {
    if (editingFret && fretInputRef.current) {
      fretInputRef.current.focus();
      fretInputRef.current.select();
    }
  }, [editingFret]);

  // Фокус на input пальца
  useEffect(() => {
    if (editingFinger && fingerInputRef.current) {
      fingerInputRef.current.focus();
      fingerInputRef.current.select();
    }
  }, [editingFinger]);

  // Хук для клика вне input (закрытие input)
  useEffect(() => {
    if (!editingFinger) return;
    function handleClickOutside(e) {
      if (
        modalRef.current &&
        !modalRef.current.contains(e.target)
      ) {
        handleFingerInputBlur("");
      }
      // Если клик вне input пальца, но внутри модала
      if (
        fingerInputRef.current &&
        !fingerInputRef.current.contains(e.target)
      ) {
        handleFingerInputBlur("");
      }
    }
    document.addEventListener("mousedown", handleClickOutside);
    return () => document.removeEventListener("mousedown", handleClickOutside);
  }, [editingFinger]);

  const handleFretNumberClick = () => {
    setEditingFret(true);
    setFretInputValue(String(startFret));
  };

  const handleFretInputBlur = () => {
    let num = parseInt(fretInputValue, 10);
    if (isNaN(num) || num < 1 || num > 20) num = 1;
    setStartFret(num);
    setApplicatura(getDefaultApplicatura(num));
    setEditingFret(false);
  };

  const handleFretInputKeyDown = (e) => {
    if (e.key === "Enter") {
      handleFretInputBlur();
    } else if (e.key === "Escape") {
      setEditingFret(false);
    }
  };

  const handleStringTopClick = (stringIdx) => {
    setApplicatura((prev) => {
      const strings = [...prev.strings];
      const hasPress = strings[stringIdx].presses.length > 0;
      const curr = strings[stringIdx].state;
      let next;
      if (!hasPress) {
        // Если нет прижатий: только open <-> muted
        next = curr === "open" ? "muted" : "open";
      } else {
        // Если есть прижатие: только null <-> muted
        next = curr === "muted" ? null : "muted";
      }
      strings[stringIdx] = { ...strings[stringIdx], state: next };
      return { ...prev, strings };
    });
  };

  const handleFretClick = (stringIdx, fretIdx) => {
    // Если по этому месту уже есть прижатие — удаляем его сразу
    setApplicatura((prev) => {
      const strings = [...prev.strings];
      const currFret = startFret + fretIdx;
      const newPresses = strings[stringIdx].presses.filter(p => p.fret !== currFret);
      strings[stringIdx] = {
        ...strings[stringIdx],
        presses: newPresses,
        state: newPresses.length > 0 ? 'pressed' : 'open',
      };
      return { ...prev, strings };
    });
    setEditingFinger({ stringIdx, fretIdx });
    setFingerInputValue("");
  };

  const handleFingerInputBlur = (fingerOverride) => {
    const { stringIdx, fretIdx } = editingFinger;
    const finger = fingerOverride !== undefined
      ? parseInt(fingerOverride, 10)
      : parseInt(fingerInputValue, 10);
  
    if (isNaN(finger) || finger < 1 || finger > 4) {
      setEditingFinger(null);
      setFingerInputValue("");
      return;
    }
  
    setApplicatura((prev) => {
      const strings = [...prev.strings];
      const currFret = startFret + fretIdx;
  
      // Собираем струны, на которых были удалены прижатия этим пальцем
      const affectedStrings = new Set();
  
      for (let i = 0; i < strings.length; i++) {
        const oldPresses = strings[i].presses;
        const newPresses = oldPresses.filter(p => !(p.finger === finger && p.fret !== currFret));
  
        if (oldPresses.length !== newPresses.length) {
          affectedStrings.add(i);
          strings[i] = {
            ...strings[i],
            presses: newPresses,
          };
        }
      }
  
      // Добавляем новое прижатие (если его ещё нет)
      const updatedPresses = strings[stringIdx].presses.filter(p => !(p.finger === finger && p.fret === currFret));
      updatedPresses.push({ fret: currFret, finger });
      strings[stringIdx] = {
        ...strings[stringIdx],
        presses: updatedPresses,
        state: "pressed",
      };
  
      // Обновляем состояние струн, с которых были удалены прижатия
      affectedStrings.forEach((i) => {
        if (strings[i].presses.length === 0) {
          strings[i] = { ...strings[i], state: "open" };
        } else {
          strings[i] = { ...strings[i], state: "pressed" };
        }
      });
  
      return { ...prev, strings };
    });
  
    setEditingFinger(null);
    setFingerInputValue("");
  };
  

  const handleFingerInputChange = (e) => {
    const val = e.target.value.replace(/\D/g, "");
    setFingerInputValue(val);
    if (["1", "2", "3", "4"].includes(val)) {
      setTimeout(() => handleFingerInputBlur(val), 0);
    }
  };

  const renderTopMarkers = () => (
    <tr>
      <td></td>
      {Array.from({ length: STRINGS_COUNT }).map((_, stringIdx) => {
        const state = applicatura.strings[stringIdx].state;
        const hasPress = applicatura.strings[stringIdx].presses.length > 0;
        let marker = null;
        if (!hasPress) {
          marker = state === "muted" ? "X" : "◯"; // только x или 0
        } else {
          marker = state === "muted" ? "X" : ""; // только x или пусто
        }
        return (
          <td key={stringIdx} className="chord-string-top-td">
            <span
              className="chord-string-top"
              onClick={() => handleStringTopClick(stringIdx)}
            >
              {marker}
            </span>
          </td>
        );
      })}
    </tr>
  );

  const renderFretNumbers = () => (
    <>
      {Array.from({ length: FRETS_COUNT }).map((_, fretIdx) => (
        <tr key={fretIdx}>
          <td
            className={"chord-fret-number" + (fretIdx === 0 ? " chord-fret-number--clickable" : "")}
            onClick={fretIdx === 0 ? handleFretNumberClick : undefined}
          >
            {editingFret && fretIdx === 0 ? (
              <input
                ref={fretInputRef}
                type="number"
                min={1}
                max={20}
                value={fretInputValue}
                onChange={e => setFretInputValue(e.target.value.replace(/\D/g, ""))}
                onBlur={handleFretInputBlur}
                onKeyDown={handleFretInputKeyDown}
                className="chord-fret-input"
              />
            ) : (
              startFret + fretIdx
            )}
          </td>
          {Array.from({ length: STRINGS_COUNT }).map((_, stringIdx) => {
            const press = applicatura.strings[stringIdx].presses.find(
              (p) => p.fret === startFret + fretIdx
            );
            const isEditing = editingFinger && editingFinger.stringIdx === stringIdx && editingFinger.fretIdx === fretIdx;
            return (
              <td
                key={stringIdx}
                className="chord-fret-cell"
                onClick={() => handleFretClick(stringIdx, fretIdx)}
              >
                {isEditing ? (
                  <input
                    ref={fingerInputRef}
                    type="number"
                    min={1}
                    max={4}
                    value={fingerInputValue}
                    onChange={handleFingerInputChange}
                    className="chord-finger-input"
                  />
                ) : press ? (
                  <span
                    className={`chord-finger-circle chord-finger-circle--color${(press.finger - 1) % 4 + 1}`}
                  >
                    {press.finger}
                  </span>
                ) : null}
              </td>
            );
          })}
        </tr>
      ))}
    </>
  );

  // Находим баррэ для текущей аппликатуры
  const barre = findBarre(applicatura);

  return (
    <div className="chord-modal-overlay">
      <div className="chord-modal" ref={modalRef}>
        <h2>Редактор аккорда</h2>
        <div className="chord-name-input-wrapper">
          <input
            type="text"
            className="chord-name-input"
            placeholder="Имя аккорда"
            value={chordName}
            onChange={e => setChordName(e.target.value)}
            maxLength={50}
            required
          />
          {chordError && <div className="chord-modal-error">{chordError}</div>}
        </div>
        <div className="chord-editor-table-wrapper">
          <table className="chord-editor-table">
            <tbody>
              {renderTopMarkers()}
              {renderFretNumbers()}
            </tbody>
          </table>
          {variationError && <div className="chord-modal-error">{variationError}</div>}
        </div>
        <div className="chord-modal-btns">
          <button onClick={onClose}>Отмена</button>
          <button
            onClick={() => onSave({
              chordName,
              applicatura: {
                strings: applicatura.strings,
              },
              startFret,
              bare: !!barre,
              fingeringSVG: generateChordSVG({ applicatura, startFret }),
            })}
          >Сохранить</button>
          <button
            type="button"
            onClick={() => {
              setChordName("");
              setStartFret(1);
              setApplicatura(getDefaultApplicatura(1));
              setFretInputValue("1");
              setEditing(null);
              setEditingFret(false);
              setEditingFinger(null);
              setFingerInputValue("");
            }}
          >Сброс</button>
        </div>
      </div>
    </div>
  );
} 