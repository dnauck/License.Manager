angular.module('AuthServices', ['ngResource']).
    factory('Auth', function($resource) {
        return $resource('api/auth/credentials', {}, {
            'login': { method: 'POST', params: { userName: '@userName', password: '@password', rememberMe: '@rememberMe' } },
            'logout': { method: 'POST', url: 'api/auth/logout' }
        });
    });