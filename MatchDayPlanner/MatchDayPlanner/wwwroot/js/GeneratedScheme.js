function changeMemberPrevention(elem) {
    var hidden_input = $(elem).find("input[type='hidden']");
    $(hidden_input).val(available_status);
}