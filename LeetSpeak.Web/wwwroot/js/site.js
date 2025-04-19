$(document).ready(function () {
    $('#logout-link').click(function (e) {
        e.preventDefault();

        if (!confirm('Are you sure you want to logout?')) return;

        $.ajax({
            url: $('#logout-form').attr('action'),
            type: 'POST',
            data: {
                __RequestVerificationToken: $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                if (response.success) {
                    window.location.href = response.redirectUrl; 
                }
            },
            error: function (xhr) {
                console.error('Logout failed:', xhr.responseText);
                $('#logout-form').submit();
            }
        });
    });
});