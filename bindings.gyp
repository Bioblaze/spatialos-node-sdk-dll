{
  'targets': [{
    'target_name': 'SpatialDLL',
    'type': 'none',
    'actions': [{
      'action_name': 'compile',
      'inputs': [ 'SpatialDLL\SDK.cs' ],
      'outputs': [ '' ],
      'message': 'msbuild SpatialDLL.sln',
      'action': ['msbuild', 'SpatialDLL.sln'],
      'conditions': [
        ['OS=="win"', {
        'message': 'Building for Windows OS',
        'action': ['msbuild', 'SpatialDLL.sln']
        }],
        ['OS=="linux" or OS=="solaris" or OS=="freebsd"', {
        'message': 'Building for non-Windows OS',
        'action': ['msbuild', 'SpatialDLL.sln']
        }]
      ]
    }]
  }]
}
