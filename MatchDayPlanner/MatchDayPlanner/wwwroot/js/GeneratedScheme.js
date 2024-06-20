function changeMemberPrevention(elem) {
    var hidden_input = $(elem).find("input[type='hidden']#prevention-status");
    $(hidden_input).val(available_status);
}