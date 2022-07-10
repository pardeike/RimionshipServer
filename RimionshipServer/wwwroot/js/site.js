(() => {
    "use strict";

    // Graphs
    const ctx = document.getElementById("myChart").getContext("2d");
    // eslint-disable-next-line no-unused-vars
    const myChart = new Chart(ctx, {
        type: "line",
        data: {
            labels: [
                "Monday",
                "Tuesday",
                "Wednesday",
                "Thursday",
                "Friday",
                "Saturday",
                "Sunday",
                "Monday",
                "Tuesday",
                "Wednesday",
                "Thursday",
                "Friday",
                "Saturday",
                "Sunday"
            ],
            datasets: [{
                label: "value or something idk",
                data: [
                    15339,
                    21345,
                    18483,
                    24003,
                    23489,
                    24092,
                    21345,
                    18483,
                    21345,
                    18483,
                    21345,
                    18483,
                    18034,
                    21345,
                    18483,
                    21345,
                    18483
                ],
                lineTension: 0,
                backgroundColor: "transparent",
                borderColor: "#007bff",
                borderWidth: 4,
                pointBackgroundColor: "#007bff"
            }]
        }
    })
})()
