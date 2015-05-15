function service($http) {
    return {
        getData: function () {
            return $http({
                method: 'GET',
                url: '/api/backend'
            });
        },
        postData: function (data) {
            return $http({
                method: 'POST',
                url: '/api/backend',
                data: data
            });
        }
    };
}