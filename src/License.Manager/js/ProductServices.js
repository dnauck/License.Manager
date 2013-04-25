angular.module('ProductServices', ['ngResource']).
    factory('Product', function($resource) {
        return $resource('api/products/:id', {}, {
            'query': { method: 'GET', isArray: false },
            'get': { method: 'GET', params: { id: '@id' } },
            'delete': { method: 'DELETE', params: { id: '@id' } }
        });
    });