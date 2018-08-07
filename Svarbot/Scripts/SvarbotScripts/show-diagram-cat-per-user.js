
$(function () {
    console.log("1");    
    $('[data-toggle="popover"]').popover();
    $('.popover-dismiss').popover({
        trigger: 'focus'
    })
    
    google.charts.load("current", { packages: ["corechart"] });
    google.charts.setOnLoadCallback(function () { drawChartCategoriesPerUser(userDetailsModel[0].UserName)});
    });
function drawChartCategoriesPerUser(username) {
    console.log("2");
    var jsonData = $.ajax({
        url: '/Admin/GetCategoriesClickedPerUser',
        type: 'GET',
        dataType: "json",
        data: {username:username},
        success: function (jsonData) {
            console.log(jsonData);
            var data = new google.visualization.DataTable(jsonData);
            data.addColumn('string', 'CategoryName');
            data.addColumn('number', 'Count');
            for (var i = 0; i < jsonData.length; i++) {
                categoryName = jsonData[i].CategoryName;
                count = jsonData[i].Count;
                data.addRow([categoryName, count]);
            }
            var options = {
                titleTextStyle: {
                    fontSize: 18,
                },
                title: 'Topp 5 mest brukte kategorier for bruker: ' + username,
                pieHole: 0.4,
                width: 500,
                height: 400,
                chartArea: { left: 50, top: 70, right: 5, bottom: 20, width: "100%", height: "100%" }
            };
            console.log(data);
            var chart = new google.visualization.PieChart(document.getElementById('chartCategPerUser'));
            chart.draw(data, options);
        }

    });
}   