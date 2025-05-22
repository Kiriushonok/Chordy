import React from "react";
import editIcon from "../assets/img/edit.svg";
import xmarkIcon from "../assets/img/xmark.svg";

const ChordFingeringBlock = ({ chord, variations, idx, onPrev, onNext, onEdit, onDelete, user, currentUserId }) => {
  const current = variations[idx];
  return (
    <div className="chord-fingering-block">
      {(current && onEdit && onDelete && (user === true || (user && current.userId === currentUserId))) && (
        <div className="chord-fingering-actions chord-fingering-actions--top">
          <button type="button" className="chord-fingering-action-btn" title="Редактировать" onClick={() => onEdit(current, chord.name)}>
            <img src={editIcon} alt="Редактировать" />
          </button>
          <button type="button" className="chord-fingering-action-btn" title="Удалить" onClick={() => onDelete(current.id, chord.name)}>
            <img src={xmarkIcon} alt="Удалить" />
          </button>
        </div>
      )}
      <div className="chord-fingering-title">{chord.name}</div>
      <div className="chord-fingering-svg-wrapper">
        {current ? (
          <div className="chord-fingering-svg" dangerouslySetInnerHTML={{ __html: current.fingeringSVG }} />
        ) : (
          <span className="chord-fingering-no-variation">Нет вариаций</span>
        )}
      </div>
      <div className="chord-fingering-nav">
        <button type="button" className="chord-fingering-arrow" onClick={onPrev}>&lt;</button>
        <span className="chord-fingering-count">{variations.length > 0 ? `${idx + 1} из ${variations.length}` : "-"}</span>
        <button type="button" className="chord-fingering-arrow" onClick={onNext}>&gt;</button>
      </div>
    </div>
  );
};

export default ChordFingeringBlock; 