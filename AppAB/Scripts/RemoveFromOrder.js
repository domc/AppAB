$(document).ready(function () {
    $(".removeLink").click(function () {
        // Get the id from the link
        var recordToDelete = $(this).attr("data-id");
        if (recordToDelete != '') {
            // Perform the ajax post
            $.post("/Orders/RemoveFromOrder", { "id": recordToDelete },
                function (data) {
                    // Successful requests get here
                    // Update the page elements
                    if (data != null)
                    {
                        if (data.itemCount == 0) {
                            $('#row-' + data.deleteId).fadeOut('slow');
                        } else {
                            $('#item-count-' + data.deleteId).text(data.itemCount);
                        }
                        $('#cart-total').text(data.totalPrice);
                        $('#update-message').text(data.message);
                    }
                });
        }
    });
});