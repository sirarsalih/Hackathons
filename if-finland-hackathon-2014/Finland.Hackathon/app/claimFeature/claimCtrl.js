function claimController($scope, claimService) {
    $scope.data = [];
    $scope.t = "tt";
    $scope.delete = function (something) {
        var index = $scope.data.indexOf(something);
        $scope.data.splice(index, 1);
    };

    claimService
        .getData({i:0})
        .success(function (data, status, headers, config) {
            displayClaimsData(data);
        });

    $scope.post = function () {
        claimService
            .postData($scope.data)
            .success(function (data, status, headers, config) {
                location.reload();
            });
    };


    var setMarkerPosition = function (map, position, sleepTime) {

        setTimeout(function () {
            var marker = new google.maps.Marker({ map: map });
            var latLng1 = new google.maps.LatLng(position.Latitude, position.Longitude);
            marker.setPosition(latLng1);
        }, sleepTime);

    };

    var displayClaimsData = function(firstdata) {

        var finland = new google.maps.LatLng(62.913586, 25.748652);
        var mapOptions = {
            zoom: 6,
            center: finland
        };
        var map = new google.maps.Map(document.getElementById('map'), mapOptions);

        var locations = JSON.parse(firstdata.Data.locations);
        for (var i = 0; i < locations.length; i++) {
            setMarkerPosition(map, locations[i], delay * i);
        }

        loadanddisplayclaims(map, 0);
    };

    var delay = 100;

    var loadanddisplayclaims = function (map, multiplier) {
        multiplier++;
        claimService.getData({ i: multiplier }).success(function (data, status, headers, config) {
            var locations = JSON.parse(data.Data.locations);
            for (var counter = 0; counter < locations.length; counter++) {
                setMarkerPosition(map, locations[counter], delay * counter + multiplier * delay * 200);
            }
            if (locations.length > 0) {
                setTimeout(function() { loadanddisplayclaims(map, multiplier); }, 0);
            }
        });
    }
}