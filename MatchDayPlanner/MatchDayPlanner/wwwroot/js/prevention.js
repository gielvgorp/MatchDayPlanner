var current_step = 0;
var available_status = "available";

function nextStep() {
    if (current_step < $(".step-wizard").length - 1) {
        $(".step-wizard").eq(current_step).fadeOut(500, function () {
            current_step++;
            $(".step-wizard").eq(current_step).fadeIn();
            $(".prevention-main-container").attr("data-current-step", current_step + 1);
            calculateBarValue();
        });
    }
}

function previousStep() {
    if (current_step > 0) {
        $(".step-wizard").eq(current_step).fadeOut(500, function () {
            current_step--;
            $(".step-wizard").eq(current_step).fadeIn();
            $(".prevention-main-container").attr("data-current-step", current_step + 1);
            calculateBarValue();
        });
    }
}

function calculateBarValue() {
    const step_amount = 100 / $(".step-wizard").length;
    const new_value = step_amount * (current_step + 1);

    $(".prevention-value").width(new_value + "%");
    $("#value-txt").text(Math.round(new_value) + "%");
}

function setAvailability(elem, schemeID) {
    var status = $(elem).attr("data-status");
    if (available_status === "") {

    }
    else if (status == available_status) {
        $(elem).removeAttr("data-status");
        setMatchday(false, $(elem).attr("data-date"), schemeID)
    }
    else {
        $(elem).attr("data-status", available_status);
        setMatchday(true, $(elem).attr("data-date"), schemeID)
    }
}

function setPrevention(add, date, schemeID) {
    $.ajax({
        url: "/prevention/SetPrevention",
        type: "post",
        data: {
            date: date,
            add: add,
            schemeID: schemeID
        },
        success: function (response) {
            console.log('test success');
        },
        error: function (jqXHR, textStatus, errorThrown) {
            console.log('test failed');
        }
    });
}