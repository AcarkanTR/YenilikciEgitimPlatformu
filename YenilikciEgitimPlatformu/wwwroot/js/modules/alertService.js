/*
 * ════════════════════════════════════════════════════════════════════════════
 * AlertService - Merkezi Kullanıcı Geri Bildirim Sistemi
 * ════════════════════════════════════════════════════════════════════════════
 * 
 * KURAL: alert(), confirm(), prompt() YASAK!
 * Tüm kullanıcı geri bildirimleri bu service üzerinden yapılır.
 * 
 * Kullanım:
 * import { AlertService } from '/js/modules/alertService.js';
 * await AlertService.confirmDelete('Silmek istediğinize emin misiniz?');
 */

export const AlertService = {

    // ════════════════════════════════════════════════════════════════════════
    // ONAY (CONFIRM) - Kritik İşlemler İçin
    // ════════════════════════════════════════════════════════════════════════

    /**
     * Silme onayı
     * @param {string} message - Onay mesajı
     * @param {string} title - Başlık (opsiyonel)
     * @returns {Promise<boolean>} Kullanıcı onayladı mı?
     */
    async confirmDelete(message = 'Bu işlem geri alınamaz!', title = 'Emin misiniz?') {
        const result = await Swal.fire({
            title: title,
            text: message,
            icon: 'warning',
            showCancelButton: true,
            confirmButtonColor: '#e94560',
            cancelButtonColor: '#64748b',
            confirmButtonText: 'Evet, Sil',
            cancelButtonText: 'İptal',
            focusCancel: true, // Yanlışlıkla silmeyi önle
            reverseButtons: true // İptal solda
        });

        return result.isConfirmed;
    },

    /**
     * Genel onay diyalogu
     * @param {string} message - Onay mesajı
     * @param {string} title - Başlık
     * @param {string} confirmText - Onay buton metni
     * @returns {Promise<boolean>}
     */
    async confirm(message, title = 'Onaylıyor musunuz?', confirmText = 'Evet') {
        const result = await Swal.fire({
            title: title,
            text: message,
            icon: 'question',
            showCancelButton: true,
            confirmButtonColor: '#0f3460',
            cancelButtonColor: '#64748b',
            confirmButtonText: confirmText,
            cancelButtonText: 'İptal',
            focusCancel: true
        });

        return result.isConfirmed;
    },

    // ════════════════════════════════════════════════════════════════════════
    // BAŞARI (SUCCESS) - İşlem Sonuçları
    // ════════════════════════════════════════════════════════════════════════

    /**
     * Toast bildirim (Küçük işlemler için - 3 saniye)
     * @param {string} message - Başarı mesajı
     */
    toastSuccess(message) {
        const Toast = Swal.mixin({
            toast: true,
            position: 'top-end',
            showConfirmButton: false,
            timer: 3000,
            timerProgressBar: true,
            didOpen: (toast) => {
                toast.addEventListener('mouseenter', Swal.stopTimer);
                toast.addEventListener('mouseleave', Swal.resumeTimer);
            }
        });

        Toast.fire({
            icon: 'success',
            title: message
        });
    },

    /**
     * Modal başarı mesajı (Kritik/büyük işlemler için)
     * @param {string} message - Başarı mesajı
     * @param {string} title - Başlık
     */
    success(message, title = 'Başarılı!') {
        Swal.fire({
            title: title,
            text: message,
            icon: 'success',
            confirmButtonColor: '#0f3460',
            confirmButtonText: 'Tamam'
        });
    },

    // ════════════════════════════════════════════════════════════════════════
    // HATA (ERROR) - Her Zaman Modal
    // ════════════════════════════════════════════════════════════════════════

    /**
     * Hata mesajı (TOAST DEĞİL, MODAL!)
     * @param {string} message - Hata mesajı
     * @param {string} title - Başlık
     */
    error(message, title = 'Bir Hata Oluştu') {
        Swal.fire({
            title: title,
            text: message,
            icon: 'error',
            confirmButtonColor: '#e94560',
            confirmButtonText: 'Tamam'
        });
    },

    /**
     * Validasyon hatası
     * @param {string} message - Hata mesajı
     */
    validationError(message) {
        this.error(message, 'Lütfen Kontrol Edin');
    },

    // ════════════════════════════════════════════════════════════════════════
    // BİLGİ (INFO) VE UYARI (WARNING)
    // ════════════════════════════════════════════════════════════════════════

    /**
     * Bilgi mesajı
     * @param {string} message - Bilgi mesajı
     * @param {string} title - Başlık
     */
    info(message, title = 'Bilgi') {
        Swal.fire({
            title: title,
            text: message,
            icon: 'info',
            confirmButtonColor: '#0f3460',
            confirmButtonText: 'Anladım'
        });
    },

    /**
     * Uyarı mesajı
     * @param {string} message - Uyarı mesajı
     * @param {string} title - Başlık
     */
    warning(message, title = 'Dikkat!') {
        Swal.fire({
            title: title,
            text: message,
            icon: 'warning',
            confirmButtonColor: '#f97316',
            confirmButtonText: 'Anladım'
        });
    },

    // ════════════════════════════════════════════════════════════════════════
    // YÜKLEME (LOADING)
    // ════════════════════════════════════════════════════════════════════════

    /**
     * Yükleme ekranı göster
     * @param {string} message - Yükleme mesajı
     */
    showLoading(message = 'İşleminiz gerçekleştiriliyor...') {
        Swal.fire({
            title: message,
            allowOutsideClick: false,
            allowEscapeKey: false,
            allowEnterKey: false,
            didOpen: () => {
                Swal.showLoading();
            }
        });
    },

    /**
     * Yükleme ekranını kapat
     */
    hideLoading() {
        Swal.close();
    }
};

// Default export
export default AlertService;