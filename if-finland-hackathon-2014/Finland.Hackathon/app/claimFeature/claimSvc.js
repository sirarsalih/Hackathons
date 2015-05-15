function claimService($http) {
    return {
        getData: function (data) {
            return $http({
                method: 'GET',
                url: '/api/claim',
                params: { i: data.i}
            });
        },
        postData: function (data) {
            return $http({
                method: 'POST',
                url: '/api/claim',
                data: data
            });
        }
    };
}