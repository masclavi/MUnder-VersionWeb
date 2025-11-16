// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
/* ========================================== */
/* SISTEMA DE VALORACIONES - AGREGADO AQUÍ */
/* ========================================== */

// Sistema de Valoraciones con Advertencia
class RatingSystem {
    constructor(songId, containerId) {
        this.songId = songId;
        this.container = document.getElementById(containerId);
        this.currentRating = 0;
        this.existingRating = 0;
        this.hasExistingReview = false;
        this.existingComment = '';

        this.init();
    }

    async init() {
        await this.loadExistingReview();
        this.render();
    }

    async loadExistingReview() {
        try {
            const response = await fetch('/api/Reviews/Song/' + this.songId);
            const data = await response.json();

            if (data.hasReview) {
                this.hasExistingReview = true;
                this.existingRating = data.rating;
                this.currentRating = data.rating;
                this.existingComment = data.comment;
            }
        } catch (error) {
            console.error('Error cargando review:', error);
        }
    }

    render() {
        const html = `
            <div class="rating-container">
                <h4>Tu Valoración</h4>
                <div class="stars-container">
                    ${this.renderStars()}
                </div>
                <div class="rating-value">${this.currentRating > 0 ? this.currentRating + ' estrella(s)' : 'Sin valorar'}</div>
                <textarea 
                    id="comment-${this.songId}" 
                    class="rating-comment" 
                    placeholder="Escribe tu comentario (obligatorio)..."
                    maxlength="500"
                >${this.existingComment}</textarea>
                <button 
                    class="btn-save-rating" 
                    onclick="ratingSystem_${this.songId}.saveRating()"
                    ${this.currentRating === 0 ? 'disabled' : ''}
                >
                    ${this.hasExistingReview ? 'Actualizar Valoración' : 'Guardar Valoración'}
                </button>
            </div>

            <!-- Modal de Advertencia -->
            <div id="warning-modal-${this.songId}" class="rating-modal" style="display: none;">
                <div class="modal-content-rating">
                    <h3>⚠️ Advertencia</h3>
                    <p id="warning-message-${this.songId}"></p>
                    <div class="modal-buttons">
                        <button class="btn-modal-cancel" onclick="ratingSystem_${this.songId}.closeWarning()">
                            Cancelar
                        </button>
                        <button class="btn-modal-confirm" onclick="ratingSystem_${this.songId}.confirmRating()">
                            Aceptar Cambio
                        </button>
                    </div>
                </div>
            </div>
        `;

        this.container.innerHTML = html;
        this.attachStarEvents();
    }

    renderStars() {
        let starsHtml = '';
        for (let i = 1; i <= 5; i++) {
            const filled = i <= this.currentRating;
            starsHtml += `
                <span 
                    class="star ${filled ? 'filled' : ''}" 
                    data-rating="${i}"
                >
                    ${filled ? '★' : '☆'}
                </span>
            `;
        }
        return starsHtml;
    }

    attachStarEvents() {
        const stars = this.container.querySelectorAll('.star');
        stars.forEach(star => {
            star.addEventListener('click', (e) => {
                const rating = parseInt(e.target.dataset.rating);
                this.setRating(rating);
            });

            star.addEventListener('mouseenter', (e) => {
                const rating = parseInt(e.target.dataset.rating);
                this.highlightStars(rating);
            });
        });

        this.container.querySelector('.stars-container').addEventListener('mouseleave', () => {
            this.highlightStars(this.currentRating);
        });
    }

    highlightStars(rating) {
        const stars = this.container.querySelectorAll('.star');
        stars.forEach((star, index) => {
            if (index < rating) {
                star.textContent = '★';
                star.classList.add('filled');
            } else {
                star.textContent = '☆';
                star.classList.remove('filled');
            }
        });
    }

    setRating(rating) {
        this.currentRating = rating;
        this.highlightStars(rating);
        this.container.querySelector('.rating-value').textContent = rating + ' estrella(s)';
        this.container.querySelector('.btn-save-rating').disabled = false;
    }

    async saveRating() {
        const comment = document.getElementById('comment-' + this.songId).value.trim();

        if (!comment) {
            this.showToast('El comentario es obligatorio', 'error');
            return;
        }

        if (this.currentRating === 0) {
            this.showToast('Debes seleccionar una calificación', 'error');
            return;
        }

        // Si ya existe una review, verificar diferencia de 3 estrellas
        if (this.hasExistingReview) {
            const difference = Math.abs(this.existingRating - this.currentRating);

            if (difference >= 3) {
                this.showWarning(difference);
                return;
            }
        }

        // Si no hay diferencia mayor a 3, guardar directamente
        await this.performSave(comment);
    }

    showWarning(difference) {
        const modal = document.getElementById('warning-modal-' + this.songId);
        const message = document.getElementById('warning-message-' + this.songId);

        message.textContent = `Estás cambiando tu valoración de ${this.existingRating} a ${this.currentRating} estrellas (diferencia de ${difference}). ¿Estás seguro de continuar?`;

        modal.style.display = 'flex';
    }

    closeWarning() {
        const modal = document.getElementById('warning-modal-' + this.songId);
        modal.style.display = 'none';

        // Restaurar rating anterior
        this.currentRating = this.existingRating;
        this.highlightStars(this.existingRating);
        this.container.querySelector('.rating-value').textContent = this.existingRating + ' estrella(s)';
    }

    async confirmRating() {
        const modal = document.getElementById('warning-modal-' + this.songId);
        modal.style.display = 'none';

        const comment = document.getElementById('comment-' + this.songId).value.trim();
        await this.performSave(comment);
    }

    async performSave(comment) {
        try {
            const response = await fetch('/api/Reviews', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify({
                    songId: this.songId,
                    rating: this.currentRating,
                    comment: comment
                })
            });

            const result = await response.json();

            if (response.ok && result.success) {
                this.hasExistingReview = true;
                this.existingRating = this.currentRating;
                this.existingComment = comment;

                this.showToast(result.message, 'success');

                // Recargar página después de 1.5 segundos
                setTimeout(function () {
                    window.location.reload();
                }, 1500);
            } else {
                this.showToast(result.message || 'Error al guardar', 'error');
            }
        } catch (error) {
            console.error('Error:', error);
            this.showToast('Error de conexión', 'error');
        }
    }
    showToast(message, type = 'info') {
        let toast = document.getElementById('rating-toast');
        if (!toast) {
            toast = document.createElement('div');
            toast.id = 'rating-toast';
            toast.className = 'rating-toast';
            document.body.appendChild(toast);
        }

        toast.textContent = message;
        toast.className = 'rating-toast show ' + type;

        setTimeout(() => {
            toast.classList.remove('show');
        }, 3000);
    }
}

// Función helper para crear una instancia
function createRatingSystem(songId, containerId) {
    window['ratingSystem_' + songId] = new RatingSystem(songId, containerId);
}