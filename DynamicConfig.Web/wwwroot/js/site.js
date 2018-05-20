  var vue = new Vue({
    el : '#app',
    data: {
      configs :null,
      name : "",
      type : "",
      value : "",
      isActive : "",
      applicationName : "",
      modalStatus : "create",
      selectedId : ""
    },
    created: function () {
      this.fetchData()

    },
    methods: {

      fetchData : function(){
        var self= this  
        fetch('/api/',{
            method: "get",
          })
          .then(x=>x.json()).then(function(data){

                self.configs = data
             
          })
      },

     createConfig: function(data){
      var self= this  
      fetch('/api/',{
        method: 'POST',
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json'
        },
        body : JSON.stringify({"Name":self.name,"Type":self.type,"Value":self.value,"IsActive":self.isActive,"ApplicationName":self.applicationName })
        
      }).then(function(data){

         self.fetchData()
      })
     },

     editPost: function(pk){
      var self= this  
      fetch('/api/'+pk,{
          method: "get",
        }).then(x=>x.json()).then(function(data){

        self.modalStatus = "update"
          $('.ui.modal')
          .modal('show');

        self.selectedId = data.Id;
        self.name= data.Name;
        self.type= data.Type;
        self.value= data.Value;
        self.isActive= data.IsActive;
        self.applicationName= data.ApplicationName
        })
     },
     deleteConfig : function(pk){
      var self= this  
      fetch('/api/'+pk,{
        method: "delete",
      }).then(function(data){
  
            self.fetchData()
      })
      
     },
     updateConfig : function(){
      var self= this  

      var data= {
      "Id":self.selectedId,
      "Name":self.name,
      "Type":self.type,
      "Value":self.value,
      "IsActive":self.isActive,
      "ApplicationName":self.applicationName 
      }

      fetch('/api/'+self.id +'/',{
        method: 'PUT',
        headers: {
          'Accept': 'application/json',
          'Content-Type': 'application/json'
        },
        body : JSON.stringify(data)
      }).then(response => {

            self.fetchData()
      })
     }
    }
  })
  


var new_config = document.querySelector("#new_config");

new_config.addEventListener('click',function(){

  $('.ui.modal')
  .modal('show');


})


