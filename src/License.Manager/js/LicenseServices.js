angular.module('LicenseServices', ['ngResource']).
    factory('License', function($resource) {
        return $resource('api/licenses/:id', { id: '@id' }, {
            'query': {
                method: 'GET',
                isArray: true,
                url: 'api/:findByEntity/:entityId/licenses',
                params: {
                    findByEntity: 'products',
                    entityId: '@entityId'
                }
            },
            'get': { method: 'GET' },
            'update': { method: 'PUT' },
            'delete': { method: 'DELETE' },
            'issue': {
                method: 'POST',
                url: 'api/licenses/issue'
            }
        });
    });