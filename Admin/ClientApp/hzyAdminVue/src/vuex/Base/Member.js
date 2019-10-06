//页面属性状态管理
export default {
    namespaced: true,
    state: {
        //Controllers 控制器名称
        controllerName: 'Member',
        //按钮状态
        buttonState: {
            insert: true,
            update: false,
            delete: false
        },
        //表格列表
        dataTable: {
            //表格转圈状态
            loading: false,
            //一页显示多少条
            rows: 20,
            //总数
            total: 0,
            //当前页码
            currentPage: 1,
            //列信息
            cols: [],
            //表格数据集合
            list: [],
            //表格复选框勾选集合
            multipleSelection: []
        },
        //检索面板表单字段
        formSearch: {
            //面板状态
            state: false,
            //模型
            vm: {
                Member_Name: null,
            }
        },
        //编辑页面表单字段状态
        form: {
            //面板状态
            state: false,
            //模型
            vm: {},
            //查找带回
            findBack: {
                state: false
            }
        }
    },
    mutations: {
        //复选框勾选改变事件
        selectionChange(state, par) {
            if (par) {
                state.dataTable.multipleSelection = par;
                state.buttonState.update = par.length == 1;
                state.buttonState.delete = par.length > 0;
            } else {
                state.dataTable.multipleSelection = [];
            }
        },
        //重置检索框数据
        resetSearch(state, par) {
            var _from = state.formSearch;
            var _vm = _from.vm;
            for (var item in _vm) _vm[item] = null;
        }
    },
    actions: {
        //获取数据表格
        findList(context, par) {
            var _state = context.state;
            _state.dataTable.loading = true;
            //收集表单数据，并组装参数
            var _formSearch = _state.formSearch;
            var _vm = _formSearch.vm;
            _vm['Page'] = _state.dataTable.currentPage;
            _vm['Rows'] = _state.dataTable.rows;
            //发送请求给接口
            global
                .post('/Admin/' + _state.controllerName + '/FindList', _vm, false)
                .then(data => {
                    var item = data;
                    _state.dataTable.loading = false;
                    _state.dataTable.rows = item.Rows;
                    _state.dataTable.total = item.TotalCount;
                    _state.dataTable.cols = item.Cols;
                    _state.dataTable.list = item.DataList;
                });
        },
        //加载表单
        loadForm(context, par) {
            var _state = context.state;
            var _multipleSelection = _state.dataTable.multipleSelection;
            var _ukid = _multipleSelection.length == 1 ? _multipleSelection[0]._ukid : null;
            if (par == 'add') _ukid = null;
            if (par == 'update' & !_ukid) return global.tools.msg('请选择要编辑的数据!', '错误');
            global
                .post('/Admin/' + _state.controllerName + '/LoadForm', { Id: _ukid }, true)
                .then(data => {
                    var item = data.Form;
                    _state.form.vm = item;
                    if (item.Id == global.tools.guidEmpty) _state.form.vm.Id = null;
                    if (par) _state.form.state = true;
                });
        },
        //删除数据
        remove(context, par) {
            var _state = context.state;
            global.tools.confirm('确定要删除吗？', function() {
                var _multipleSelection = _state.dataTable.multipleSelection;
                var _ukids = [];
                _multipleSelection.forEach(item => _ukids.push(item._ukid));
                global
                    .post('/Admin/' + _state.controllerName + '/Delete', { Id: _ukids }, true)
                    .then(data => {
                        //刷新列表
                        context.dispatch("findList");
                        if (par) par(); //执行回调
                        global.tools.msg('操作成功!', '成功');
                    });
            }, null, '警告');
        },
        //保存数据
        save(context, par) {
            var _state = context.state;
            //收集表单数据，并组装参数
            var _form = _state.form;
            var _vm = _form.vm;
            //验证数据
            if (!_vm.User_Name) return global.tools.msg('用户名不能为空!', '错误');
            //发送请求给接口
            global
                .post('/Admin/' + _state.controllerName + '/Save', _vm, true)
                .then(data => {
                    //刷新列表
                    context.dispatch("findList");
                    _form.state = false;
                    global.tools.msg('操作成功!', '成功');
                });
        },
    },
    getters: {}
}