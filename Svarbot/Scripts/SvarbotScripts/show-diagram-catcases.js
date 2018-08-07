$(function () {
    $('[data-toggle="popover"]').popover();
    $('.popover-dismiss').popover({
        trigger: 'focus'
    })
    $("#casedetailstable").DataTable();
    google.charts.load("current", { packages: ["corechart"] });
    google.charts.setOnLoadCallback(drawChartDonut);
});
function drawChartDonut() {

    var jsonData = $.ajax({
        url: '/Admin/GetCategoriesWithCases',
        type: 'GET',
        dataType: "json",
        success: function (jsonData) {
            console.log(jsonData);
            var data = new google.visualization.DataTable(jsonData);
            data.addColumn('string', 'CategoryName');
            data.addColumn('number', 'CasesPerCategory');
            for (var i = 0; i < jsonData.length; i++) {
                categoryName = jsonData[i].CategoryName;
                casesPerCategory = jsonData[i].CasesPerCategory;
                data.addRow([categoryName, casesPerCategory]);
            }
            var options = {
                titleTextStyle: {
                    fontSize: 18,
                },
                width: 550,
                height: 450,
                pieHole: 0.4,
                chartArea: { left: 50, top: 70, right: 5, bottom: 20, width: "100%", height: "100%" }
            };
            options.title = 'Antall saker per en kategori';
            console.log(data);
            var chart = new google.visualization.PieChart(document.getElementById('chartCases'));
            chart.draw(data, options);
        }

    });
}   