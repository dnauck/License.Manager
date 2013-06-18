function LoginCtrl($scope, $location, $log, $window, Auth, authInterceptorService) {

    $scope.notificationAlert = { show: false, message: '', type: 'info' };
    
    $scope.defaultAuthData = { rememberMe: false, openIdUrl: '' };
    $scope.authData = angular.copy($scope.defaultAuthData);

    $scope.login = function(loginData) {

        if (!loginData.rememberMe)
            loginData.rememberMe = false;

        Auth.login(loginData,
            function(success, getResponseHeaders) {
                $scope.notificationAlert.show = false;
                authInterceptorService.loginConfirmed();
            },
            function (error, getResponseHeaders) {
                $scope.authData = angular.copy($scope.defaultAuthData);
                $scope.notificationAlert.show = true;
                $scope.notificationAlert.type = 'error';
                $scope.notificationAlert.message = error.data.responseStatus.message;
            });
    };

    $scope.signInWithGoogle = function() {
        $window.location = '/api/auth/googleopenid';
    };
    
    $scope.signInWithYahoo = function() {
        $window.location = '/api/auth/yahooopenid';
    };
    
    $scope.signInWithMyOpenId = function () {
        $window.location = '/api/auth/myopenid';
    };
    
    $scope.signInWithOpenId = function (openIdUrl) {
        $window.location = '/api/auth/openid?OpenIdUrl=' + openIdUrl;
    };
    
    $scope.$on('event:auth-loginRequired', function (event, response) {
        $scope.notificationAlert.show = true;
        $scope.notificationAlert.type = 'error';
        $scope.notificationAlert.message = response.data.responseStatus.message;
        $scope.authData = angular.copy($scope.defaultAuthData);
    });
}

//LoginCtrl.$inject = ['$scope', '$location', '$log', 'Auth', 'authInterceptorService'];