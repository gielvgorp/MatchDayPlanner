var available_status = "";
$(document).ready(function () {
    generateCalendar(2, 4);

    $(".legenda-item").on("click", function () {
        var status = $(this).data("status");
        if (status == available_status) {
            available_status = "";
            $(this).removeAttr("selected");
        } else {
            $(".legenda-item").removeAttr("selected");
            available_status = $(this).data("status");
            $(this).attr("selected", "");
        }
       
    })
});

// generates calendar based on start month and many months
function generateCalendar(startIndex, amount) {
    for (var z = 0; z < amount; z++) {
        // Create a new Date object
        var currentDate = new Date(2024, startIndex + z);

        // Array of month names
        var monthNames = ["Januari", "Februari", "Maart", "April", "Mei", "Juni",
            "Juli", "Augustus", "September", "Oktober", "November", "December"];

        var call_month = document.createElement("div");
        call_month.classList.add("cal-month");

        // Get month name based on index
        var currentMonthIndex = currentDate.getMonth();
        var currentMonthName = monthNames[currentMonthIndex];

        // create and set month name
        var monthHeader = document.createElement("h3");
        monthHeader.className = "text cal-month-name";

        monthHeader.textContent = currentMonthName + " " + currentDate.getFullYear();
        call_month.appendChild(monthHeader);

        var cal_days = document.createElement('div');
        cal_days.classList.add("cal-days");

        var number_of_month = getDaysOfMonth(currentDate.getFullYear(), currentMonthIndex + 1);
        var first_day_week_number = getDayOfWeek(currentDate.getFullYear(), currentDate.getMonth(), 1);

        for (var i = 0; i < first_day_week_number; i++) {
            var day_cell = document.createElement('div');
            day_cell.className = "day-cell-disabled cal-day-cell";
            cal_days.appendChild(day_cell);
        }


        for (var i = 0; i < number_of_month; i++) {
            var day_cell = document.createElement('div');
            day_cell.classList.add("cal-day-cell");

            var day_cell_heading = document.createElement('h5');
            day_cell_heading.classList.add("text");
            day_cell_heading.textContent = i + 1;
            day_cell.appendChild(day_cell_heading);

            // add event on click to day cell
            day_cell.onclick = function () {
                var status = $(this).attr("data-status");
                if (available_status === "") {
                    
                }
                else if (status == available_status) $(this).removeAttr("data-status");
                else $(this).attr("data-status", available_status);
            };

            cal_days.appendChild(day_cell);
        }
        call_month.appendChild(cal_days);

        // Append the div to the body of the document
        document.getElementsByClassName("cal-grid")[0].appendChild(call_month);
    }
}

function getDaysOfMonth(year, month) {
    return new Date(year, month, 0).getDate();
}

function getDayOfWeek(year, month, day) {
    var date = new Date(year, month - 1, day);
    var dayOfWeek = date.getDay();

    return dayOfWeek;
}