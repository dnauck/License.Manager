var app =
    angular.module('license-manager', ['http-auth-interceptor', 'AuthServices', 'CustomerServices', 'ProductServices']).
        config(['$routeProvider', '$locationProvider',
            function($routeProvider, $locationProvider) {
                $routeProvider.
                    when('/customers/add', { templateUrl: 'partials/customer-add.html', controller: CustomerAddCtrl }).
                    when('/customers', { templateUrl: 'partials/customer-list.html', controller: CustomerListCtrl }).
                    when('/customers/:id', { templateUrl: 'partials/customer-detail.html', controller: CustomerDetailsCtrl }).
                    when('/login', { templateUrl: 'partials/login.html', controller: LoginCtrl }).
                    when('/', { templateUrl: 'partials/login.html', controller: LoginCtrl }).
                    otherwise({ redirectTo: '/' });

                $locationProvider.html5Mode(false).hashPrefix('!');
            }]);

app.controller('AppController', function ($scope, $rootScope, $location, $log, Auth) {

    $scope.isCurrentMainMenuItem = function(menuItem) {
        return $location.path().split('/')[1] === menuItem;
    };

    $scope.logout = function() {
        Auth.logout();
    };
    
    $rootScope.$on('event:auth-loginRequired', function() {
        $rootScope.authReferrerUrl = $location.path();
        $log.error('Error - event:auth-loginRequired on path ' + $rootScope.authReferrerUrl);
        $location.path('/login');
    });

    $rootScope.$on('event:auth-loginConfirmed', function() {
        $log.log('Success - event:auth-loginConfirmed on path ' + $location.path() + ' with ref Url ' + $rootScope.authReferrerUrl);

        if ($rootScope.authReferrerUrl != undefined)
            $location.path($rootScope.authReferrerUrl);

        $location.path('/');
    });
});