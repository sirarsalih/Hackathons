var app = angular.module("finland.hackathon", []);


app.controller("claimController", claimController);

app.factory("claimService", claimService);

app.directive("directive", directive);