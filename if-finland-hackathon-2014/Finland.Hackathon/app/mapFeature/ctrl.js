/// <reference path="ctrl.js" />
function controller($scope, service) {
    $scope.claims = [];
    $scope.helsinkiAirport = new google.maps.LatLng(60.321177, 24.952860);
    $scope.startupSauna = new google.maps.LatLng(60.180805, 24.832412);
    $scope.directionsDisplay = new google.maps.DirectionsRenderer();
    $scope.waypoints = [
        {
            location: new google.maps.LatLng(60.172030, 24.941433),
            stopover: true
        }
    ];
    $scope.positions = [];
    $scope.startPosition = 0;
    $scope.endPosition = 0;

    $scope.zoomLevel = 14;

    $scope.showFood = true;
    $scope.isDriving = false;
    $scope.isEnRoute = false;
    $scope.showInfoMessage = false;

    $scope.isFirstClaim  = true;
    service
        .getData()
        .success(function(data, status, headers, config) {
            $scope.claims = data.Data.claims;
            drawMarkersOnGoogleMapsAndCalculateRoute();
        });

    $scope.post = function() {
        service
            .postData($scope.claims)
            .success(function(data, status, headers, config) {
                location.reload();
            });
    };

    $scope.route1 = function() {
        var request = {
            origin: $scope.helsinkiAirport,
            destination: $scope.startupSauna,
            waypoints: $scope.waypoints,
            travelMode: google.maps.TravelMode.DRIVING
        };
        $scope.calculateRoute(request, $scope.directionsDisplay);
    }

    $scope.route2 = function() {
        var request = {
            origin: $scope.helsinkiAirport,
            destination: $scope.startupSauna,
            travelMode: google.maps.TravelMode.DRIVING
        };
        $scope.calculateRoute(request, $scope.directionsDisplay);
    }

    var drawMarkersOnGoogleMapsAndCalculateRoute = function() {
        var mapOptions = {
            zoom: 17,
            center: $scope.helsinkiAirport
        };
        $scope.map = new google.maps.Map(document.getElementById('map'), mapOptions);
        $scope.directionsDisplay.setMap($scope.map);

        var trafficLayer = new google.maps.TrafficLayer();
        trafficLayer.setMap($scope.map);
        $scope.route1();

        var image = {
            url: '/images/car.png',
            anchor: new google.maps.Point(16, 16)
        };
        $scope.marker = new google.maps.Marker({ map: $scope.map, icon: image });
    }

    $scope.calculateRoute = function(request, directionsDisplay) {
        var directionsService = new google.maps.DirectionsService();
        directionsService.route(request, function(response, status) {
            if (status == google.maps.DirectionsStatus.OK) {
                directionsDisplay.setDirections(response);
                console.info(response);
                $scope.positions = response.routes[0].overview_path;
                var latLng = new google.maps.LatLng($scope.positions[0].k, $scope.positions[0].B);
                $scope.marker.setPosition(latLng);
            }
        });
    }

    $scope.drivePositions = function (start, end) {
    	if (!$scope.isDriving) {
    		$scope.showInfoMessage = false;
    	}

        $scope.showFood = false;
        $scope.isDriving = true;
        $scope.isEnRoute = true;
        
        $scope.map.setZoom($scope.zoomLevel);
        $scope.startPosition = start;
        $scope.endPosition = end;

        var sleepIndex = 0;
        for (var i = start; i < end; i++) {
            moveMarkerPositionSlowlyAndDrawInfoWindows($scope.positions[i], sleepIndex * 100, i);
            sleepIndex++;
        }
    }

    var moveMarkerPositionSlowlyAndDrawInfoWindows = function(currentPosition, sleepTime, currentPositionIndex) {
            setTimeout(function() {
                var latLng = new google.maps.LatLng(currentPosition.k, currentPosition.B);
                $scope.map.setCenter(latLng);
                $scope.marker.setPosition(latLng);
                checkIfCurrentPositionIsCloseToClaimsPositionAndDrawInfoWindows(currentPositionIndex, 20);
                //TODO: Keep this function call, it's handy -Sirar
                //checkIfCurrentPositionIsSameAsClaimsPositionAndDrawInfoWindows(currentPosition);
            }, sleepTime);
        };

    var checkIfCurrentPositionIsCloseToClaimsPositionAndDrawInfoWindows = function (positionIndex, numOfDistancePoints) {
    	//console.log("checking if current position is close to claims position and draws info windows");
            for (var j = 0; j < $scope.claims.length; j++) {
                if ($scope.positions[positionIndex + numOfDistancePoints] != undefined) {
                    if ($scope.positions[positionIndex + numOfDistancePoints].k == $scope.claims[j].latitude && $scope.positions[positionIndex + numOfDistancePoints].B == $scope.claims[j].longitude) {
                    	var currentClaimLocation = $scope.claims[j];
                    	drawClaimInfoWindow($scope.map, currentClaimLocation);
                        
                        $scope.$apply(function () {
                        	$scope.showInfoMessage = true;

                        	if (currentClaimLocation.triggerStop) {
                        		$scope.isDriving = false;
                        	}
                        });
                    }
                }
            }
        }

        //TODO: Keep this function, it's handy -Sirar
        var checkIfCurrentPositionIsSameAsClaimsPositionAndDrawInfoWindows = function(position) {
            var claims = $scope.claims;
            for (var i = 0; i < claims.length; i++) {
                if (position.k == claims[i].latitude && position.B == claims[i].longitude) {
                    drawClaimInfoWindow($scope.map, $scope.claims[i]);
                }
            }
        }

        var drawClaimInfoWindow = function(map, claim) {
            var latLng = new google.maps.LatLng(claim.latitude, claim.longitude);
            drawInfoWindow(map, latLng, claim.caption, claim.text);
        }

        var drawInfoWindow = function (map, latLng, headerText, contentText) {
        	console.log(headerText + " " + contentText);
            var infoHeader = document.getElementById('info-header');
            var infoContent = document.getElementById('info-text');
            console.log(infoContent);
            infoHeader.innerHTML = headerText;
            infoContent.innerHTML = contentText;
            new google.maps.Marker({
                position: latLng,
                map: map
            });
        }
    }
