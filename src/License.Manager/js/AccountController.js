function AccountListCtrl($scope, $location, Account) {

    $scope.notificationAlert = { show: false, message: '', type: 'info' };

    $scope.accounts = Account.query(
        function (success, getResponseHeaders) {
            $scope.notificationAlert.show = false;
        },
        function (error, getResponseHeaders) {
            $scope.notificationAlert.show = true;
            $scope.notificationAlert.type = 'error';
            $scope.notificationAlert.message = error.data.responseStatus.message;
        });

    $scope.deleteAccount = function (account) {
        Account.delete({ id: account.id },
            function (success, getResponseHeaders) {
                $scope.accounts.splice($scope.accounts.indexOf(account), 1);
                $scope.notificationAlert.show = false;
            },
            function (error, getResponseHeaders) {
                $scope.notificationAlert.show = true;
                $scope.notificationAlert.type = 'error';
                $scope.notificationAlert.message = error.data.responseStatus.message;
            });
    };
}

//AccountListCtrl.$inject = ['$scope', '$location', 'Customer'];

function AccountDetailsCtrl($scope, $routeParams, Account) {

    $scope.notificationAlert = { show: false, message: '', type: 'info' };

    $scope.id = $routeParams.id;
    $scope.account = Account.get({ id: $scope.id },
        function(success, getResponseHeaders) {
        },
        function(error, getResponseHeaders) {
            $scope.notificationAlert.show = true;
            $scope.notificationAlert.type = 'error';
            $scope.notificationAlert.message = error.data.responseStatus.message;
        });

    $scope.updateAccount = function(account) {
        $scope.account = Account.update(account,
            function(success, getResponseHeaders) {
                $scope.notificationAlert.show = true;
                $scope.notificationAlert.type = 'success';
                $scope.notificationAlert.message = 'Successfuly updated!';
            },
            function(error, getResponseHeaders) {
                $scope.notificationAlert.show = true;
                $scope.notificationAlert.type = 'error';
                $scope.notificationAlert.message = error.data.responseStatus.message;
            });
    };
}

//AccountDetailsCtrl.$inject = ['$scope', '$routeParams', 'Customer'];