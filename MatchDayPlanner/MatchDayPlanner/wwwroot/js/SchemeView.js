function nextStep(schemeID) {
    const uid = $("#player-select").val();
    $("#feedback-memberid").val(uid);

    $.ajax({
        url: "/prevention/GetSchemeOfMember",
        type: "post",
        data: {
            schemeID: schemeID,
            playerID: uid
        },
        success: function (response) {
            response = response.value;

            if (response != null) {
                $("#wizard").fadeOut(500, function () {
                    console.log(response);
                    for (var i = 0; i < response.memberScheme.length; i++) {
                        const dateTimeString = response.memberScheme[i].matchday.matchDayDateTime;
                        const dateObj = new Date(dateTimeString);

                        var month = dateObj.getMonth() + 1;
                        var day = dateObj.getDate();
                        var year = dateObj.getFullYear();

                        if (month < 10)
                            month = "0" + month;

                        if (day < 10)
                            day = "0" + day;

                        const formattedDate = `${month}/${day}/${year}`;
                        console.log(formattedDate);

                        var result = $(".cal-day-cell[data-date='" + formattedDate + "']");
                        console.log(result);

                        if (result) {
                            switch (response.memberScheme[i].member.memberPrevention) {
                                case 0:
                                    $(result).attr("data-status", "unavailable");
                                    break;
                                case 1:
                                    $(result).attr("data-status", "available");
                                    break;
                                case 1:
                                    $(result).attr("data-status", "available");
                                    break;
                                case 3:
                                case 4:
                                    $(result).attr("data-status", "scheduled");
                                    break;
                            }
                        }

                        $("#cal-grid").fadeIn(500);
                    }
                });
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            error(textStatus);
            //console.error('FAILED: Something went wrong while fetching player availablity!');
        }
    });


}