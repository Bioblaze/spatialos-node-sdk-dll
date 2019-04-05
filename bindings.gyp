{
  'targets': [{
    'target_name': 'SpatialDLL',
    'type': 'none',
    'actions': [{
      'action_name': 'compile',
      'inputs': [ 'SpatialDLL\SDK.cs' ],
      'outputs': [ '' ],
      'message': 'msbuild \SpatialDLL\SpatialDLL.csproj',
      'action': ['msbuild', '\SpatialDLL\SpatialDLL.csproj', '/p:Configuration=Release'],
      'conditions': [
        ['OS=="win"', {
        'message': 'Building for Windows OS',
        'action': ['msbuild', '\SpatialDLL\SpatialDLL.csproj', '/p:Configuration=Release']
        }],
        ['OS=="linux" or OS=="solaris" or OS=="freebsd"', {
        'message': 'Building for non-Windows OS',
        'action': ['msbuild', '/p:Configuration=Release', '\SpatialDLL\SpatialDLL.csproj']
        }]
      ]
    }]
  }]
}
