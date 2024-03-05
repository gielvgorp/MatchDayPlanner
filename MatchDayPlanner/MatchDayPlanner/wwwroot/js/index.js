$(document).ready(function () {
    $(".login-head > div").on("click", function () {
        var type = $(this).data("type");
        $(".head-strip").attr("data-selected", type);

        if (type === "signup") {
            $("#signin-form").fadeOut(500);
            $("#signup-form").delay(600).fadeIn(500);
        } else {
            $("#signup-form").fadeOut(500);
            $("#signin-form").delay(600).fadeIn(500);
        }
    });
})