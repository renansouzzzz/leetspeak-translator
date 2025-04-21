$(document).ready(function () {
    let currentPage = 1;
    const pageSize = 10;

    loadResults(currentPage);

    $('#searchForm').submit(function (e) {
        e.preventDefault();
        currentPage = 1;
        loadResults(currentPage);
    });

    $('button[type="reset"]').click(function () {
        currentPage = 1;
        setTimeout(() => loadResults(currentPage), 100);
    });

    function loadResults(page) {
        const formData = $('#searchForm').serializeArray();
        const requestData = {
            pageNumber: page,
            pageSize: pageSize
        };

        formData.forEach(item => {
            if (item.value) {
                requestData[item.name] = item.name.includes('Date')
                    ? new Date(item.value).toISOString()
                    : item.value;
            }
        });

        showLoading(true);

        $.ajax({
            url: '/api/translations/history',
            type: 'GET',
            data: requestData,
            success: function (data) {
                renderResults(data.data);
                renderPagination(data.data.length, page);
            },
            error: function (xhr) {
                console.error('Error fetching history:', xhr.responseText);
                toastr.error('Failed to load translation history');
            },
            complete: function () {
                showLoading(false);
            }
        });
    }

    function renderResults(translations) {
        const $tbody = $('#resultsBody');
        $tbody.empty();

        if (translations.length === 0) {
            $tbody.append(`
                <tr>
                    <td colspan="3" class="text-center py-4">
                        <i class="fas fa-info-circle me-2"></i>No translations found
                    </td>
                </tr>
            `);
            return;
        }

        translations.forEach(translation => {
            $tbody.append(`
                <tr>
                    <td>${escapeHtml(translation.originalText)}</td>
                    <td>${escapeHtml(translation.translatedText)}</td>
                    <td>${new Date(translation.translationDate).toLocaleString()}</td>
                </tr>
            `);
        });
    }

    function renderPagination(totalItems, currentPage) {
        const $pagination = $('#pagination');
        $pagination.empty();

        if (totalItems < pageSize) return;

        const totalPages = Math.ceil(totalItems / pageSize);
        const maxVisiblePages = 5;
        let startPage, endPage;

        if (totalPages <= maxVisiblePages) {
            startPage = 1;
            endPage = totalPages;
        } else {
            const maxPagesBeforeCurrent = Math.floor(maxVisiblePages / 2);
            const maxPagesAfterCurrent = Math.ceil(maxVisiblePages / 2) - 1;

            if (currentPage <= maxPagesBeforeCurrent) {
                startPage = 1;
                endPage = maxVisiblePages;
            } else if (currentPage + maxPagesAfterCurrent >= totalPages) {
                startPage = totalPages - maxVisiblePages + 1;
                endPage = totalPages;
            } else {
                startPage = currentPage - maxPagesBeforeCurrent;
                endPage = currentPage + maxPagesAfterCurrent;
            }
        }

        const prevDisabled = currentPage === 1 ? 'disabled' : '';
        $pagination.append(`
            <li class="page-item ${prevDisabled}">
                <a class="page-link" href="#" aria-label="Previous" ${prevDisabled ? 'tabindex="-1"' : ''}>
                    <span aria-hidden="true">&laquo;</span>
                </a>
            </li>
        `);

        for (let i = startPage; i <= endPage; i++) {
            const active = i === currentPage ? 'active' : '';
            $pagination.append(`
                <li class="page-item ${active}">
                    <a class="page-link" href="#">${i}</a>
                </li>
            `);
        }

        const nextDisabled = currentPage === totalPages ? 'disabled' : '';
        $pagination.append(`
            <li class="page-item ${nextDisabled}">
                <a class="page-link" href="#" aria-label="Next" ${nextDisabled ? 'tabindex="-1"' : ''}>
                    <span aria-hidden="true">&raquo;</span>
                </a>
            </li>
        `);

        $pagination.find('a').click(function (e) {
            e.preventDefault();
            const text = $(this).text().trim();
            if (text === '«') {
                currentPage--;
            } else if (text === '»') {
                currentPage++;
            } else {
                currentPage = parseInt(text);
            }
            loadResults(currentPage);
        });
    }

    function showLoading(show) {
        if (show) {
            $('#resultsBody').html(`
                <tr>
                    <td colspan="3" class="text-center py-4">
                        <div class="spinner-border text-primary" role="status">
                            <span class="visually-hidden">Loading...</span>
                        </div>
                    </td>
                </tr>
            `);
        }
    }

    function escapeHtml(unsafe) {
        return unsafe
            ? unsafe.toString()
                .replace(/&/g, "&amp;")
                .replace(/</g, "&lt;")
                .replace(/>/g, "&gt;")
                .replace(/"/g, "&quot;")
                .replace(/'/g, "&#039;")
            : '';
    }
});