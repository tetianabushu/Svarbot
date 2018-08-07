function toggleFavorite(subcategoryId) {
    var subcategory = $("#subcategory" + subcategoryId);
    $.ajax({
        url: '/Home/ToggleFavorite',
        type: 'POST',
        data: { subcategoryId: subcategoryId },
        success: function () {
            if (subcategory.hasClass('categoryStarred')) {
                subcategory.removeClass('categoryStarred');
                subcategory.addClass('categoryUnstarred');
            }
            else {
                subcategory.removeClass('categoryUnstarred');
                subcategory.addClass('categoryStarred');
            }
        },
        error: function () {
            alert("Feil ved oppdatering av favoritt");
        }
    });
}