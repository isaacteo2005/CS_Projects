import React from 'react';
import './ConfirmDialog.css';

// resuseable confirmation dialog component
function ConfirmDialog({ 
    isOpen, 
    onClose, 
    onConfirm, 
    title, 
    message,
    confirmText = 'Delete',
    cancelText = 'Cancel',
    confirmButtonClass = 'confirm-danger'
}) {
    if (!isOpen) return null;

    return (
        <div className="confirm-overlay">
            <div className="confirm-dialog">
                <h3>{title}</h3>
                <p>{message}</p>
                <div className="confirm-buttons">
                    <button 
                        className="confirm-cancel"
                        onClick={onClose}
                    >
                        {cancelText}
                    </button>
                    <button 
                        className={`confirm-action ${confirmButtonClass}`}
                        onClick={() => {
                            onConfirm();
                            onClose();
                        }}
                    >
                        {confirmText}
                    </button>
                </div>
            </div>
        </div>
    );
}

export default ConfirmDialog;