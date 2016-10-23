app = angular.module("flightApp", []);
app.controller("ScheduleController", function ($scope, $http, $filter) {
    var message = "Oops! Sorry! Something went wrong. Please contact support.";
    $scope.statusList = [];
    $scope.statusList.push('Scheduled', 'Arrived', 'Departed', 'Delayed', 'Cancelled');
    prepareDiplayMessage = function (data) {
        var msg = "Sucess!!! Flight(s) that modified:\nGate-Flight Number\n";

        angular.forEach(data, function (item) {
            msg = msg + item.Gate + '-' + item.FlightNumber + '\n';
        });
        alert(msg);
    }
    $scope.resetFields = function (obj) {
        obj.gate = null;
        obj.flightno = null;
        obj.arrival = null;
        obj.departure = null;
        obj.status = null;
    }

    var getSchedule = function () {
        var res = $http.get("http://localhost:65069/api/schedule");

        res.success(function (data, status, headers, config) {
            $scope.schedulesSearch = data;
        });
        res.error(function (data, status, headers, config) {
            alert(message);
            console.log(JSON.stringify({ data: data }));
        });
    }

    $scope.getSchedulesSearch = function () {
        var res;

        if ($scope.searchGate == null || $scope.searchGate == 'All') {
            res = $http.get("http://localhost:65069/api/schedule");
        }
        else {
            res = $http.get("http://localhost:65069/api/schedule?Gate=" + $scope.searchGate);
        }

        res.success(function (data, status, headers, config) {
            $scope.schedulesSearchResult = data;
        });
        res.error(function (data, status, headers, config) {
            alert(message);
            console.log(JSON.stringify({ data: data }));
        });
    }

    $scope.loadSchedules = function () {
        var res = $http.get("http://localhost:65069/api/schedule");

        res.success(function (data, status, headers, config) {
            $scope.schedules = data;
        });
        res.error(function (data, status, headers, config) {
            alert(message);
            console.log(JSON.stringify({ data: data }));
        });
    }


    $scope.getSchedulesPerGate = function () {
        var res = $http.get("http://localhost:65069/api/schedule?Gate=" + $scope.flight.distinctGate);

        res.success(function (data, status, headers, config) {
            $scope.schedulesPerGate = data;
        });
        res.error(function (data, status, headers, config) {
            alert(message);
            console.log(JSON.stringify({ data: data }));
        });
    }

    $scope.getTime = function () {
        var myFlight = $filter('filter')($scope.schedulesPerGate, { FlightNumber: $scope.flight.flightno })[0];
        $scope.flight.gate = myFlight.Gate;
        $scope.flight.arrival = myFlight.Arrival;
        $scope.flight.departure = myFlight.Departure;
        $scope.flight.status = myFlight.Status;
    }

    $scope.addSchedule = function () {
        var schedule = {
            Gate: $scope.addFlight.gate,
            FlightNumber: $scope.addFlight.flightno,
            Arrival: $scope.addFlight.arrival,
            Departure: $scope.addFlight.departure,
            Status: $scope.addFlight.status
        }

        var res = $http.post('http://localhost:65069/api/schedule', schedule);
        res.success(function (data, status, headers, config) {
            $scope.searchGate = 'All';
            $scope.getSchedulesSearch();
            prepareDiplayMessage(data);
            $scope.resetFields($scope.addFlight);
        });
        res.error(function (data, status, headers, config) {
            alert(message);
            console.log(JSON.stringify({ data: data }));
        });

    }



    $scope.updateSchedule = function () {
        var schedule = {
            Gate: $scope.flight.gate,
            FlightNumber: $scope.flight.flightno,
            Arrival: $scope.flight.arrival,
            Departure: $scope.flight.departure,
            Status: $scope.flight.status
        }

        var res = $http.put('http://localhost:65069/api/schedule', schedule);
        res.success(function (data, status, headers, config) {
            $scope.getSchedulesSearch();
            prepareDiplayMessage(data);
        });
        res.error(function (data, status, headers, config) {
            alert(message);
            console.log(JSON.stringify({ data: data }));
        });

        $scope.resetFields($scope.flight);
    }

    getSchedule();
    $scope.getSchedulesSearch();
    $scope.loadSchedules();
});



app.filter('distinct', function () {
    return function (collection, keyname) {
        var output = [],
            keys = [];

        angular.forEach(collection, function (item) {
            var key = item[keyname];
            if (keys.indexOf(key) === -1) {
                keys.push(key);
                output.push(item);
            }
        });

        return output;
    };
});