var current_step = 0;
var available_status = "available";

function nextStep() {
    if (current_step < $(".step-wizard").length - 1) {
        $(".step-wizard").eq(current_step).fadeOut(500, function () {
            current_step++;
            $(".step-wizard").eq(current_step).fadeIn();
            $(".prevention-main-container").attr("data-current-step", current_step + 1);
            calculateBarValue();
            updateButtonText();
            handleActionOnStepChange();
        });
    } else {
        current_step++;
        handleActionOnStepChange();
    }
}

function previousStep() {
    if (current_step > 0) {
        $(".step-wizard").eq(current_step).fadeOut(500, function () {
            current_step--;
            $(".step-wizard").eq(current_step).fadeIn();
            $(".prevention-main-container").attr("data-current-step", current_step + 1);
            calculateBarValue();
            updateButtonText();
            handleActionOnStepChange();
        });
    }
}

function updateButtonText() {
    if (current_step == $(".step-wizard").length - 1) {
        $("#prevention-next-btn").text("Verstuur");
    } else {
        $("#prevention-next-btn").text("Volgende");
    }
}

function handleActionOnStepChange() {
    switch (current_step) {
        case 0: // begin step
            console.log("begin step");
            break;
        case 1: // set prevention
            console.log("prevention step");
            break;
        case 2: // set comment
            console.log("comment step");
            break;
        case 3:
            $("#prevention-memberID").val($("#player-select").val());
            $("#prevention-comment-form").submit();
            break;
    }
}

function calculateBarValue() {
    const step_amount = 100 / $(".step-wizard").length;
    const new_value = step_amount * (current_step + 1);

    $(".prevention-value").width(new_value + "%");
    $("#value-txt").text(Math.round(new_value) + "%");
}

function getAvailabilityOfSelectedPlayer(schemeID) {
    const playerID = $("#player-select").val();
    $.ajax({
        url: "/prevention/GetPrevention",
        type: "post",
        data: {
            schemeID: schemeID,
            playerID, playerID
        },
        success: function (response) {
            $(response.value).each(function (index, value) {
                var result = $(".cal-day-cell[data-matchday='" + value.matchDayID + "']");

                if (result) {
                    if (value.isPresent) {
                        $(result).attr("data-status", "available");
                    } else {
                        $(result).attr("data-status", "unavailable");
                    }
                }
            })
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('FAILED: Something went wrong while fetching player availablity!');
        }
    });
}

function setAvailability(elem, schemeID) {
    const playerID = $("#player-select").val();
    var status = $(elem).attr("data-status");
    if (available_status === "") {

    }
    else if (status == available_status) {
        $(elem).removeAttr("data-status");
        setPrevention(false, $(elem).attr("data-date"), schemeID, playerID)
    }
    else {
        $(elem).attr("data-status", available_status);
        setPrevention(true, $(elem).attr("data-date"), schemeID, playerID)
    }
}

function setPrevention(add, date, schemeID, playerID) {
    $.ajax({
        url: "/prevention/SetPrevention",
        type: "post",
        data: {
            date: date,
            add: add,
            schemeID: schemeID,
            playerID: playerID,
            status: available_status
        },
        success: function (response) {
            console.log('test success');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('FAILED: something went wrong while update prevention!');
        }
    });
}

function getPreventionComment(schemeID) {
    const memberID = $("#player-select").val();
    $.ajax({
        url: "/prevention/GetPreventionComment",
        type: "post",
        data: {
            schemeID: schemeID,
            playerID: memberID
        },
        success: function (response) {
            response = response.value[0];
            if (response != undefined) {
                $("#prevention-comment-input").val(response.comment.trim());
            } else {
                $("#prevention-comment-input").val("");
            }
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.error('FAILED: Something went wrong while fetching prevention comment!');
        }
    });
}