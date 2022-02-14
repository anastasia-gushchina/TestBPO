function saveName(){
    sessionStorage.setItem("name",document.getElementsByTagName("input")[0].value);
    window.location.href='result.html';
}

function check(){
    let xhr = new XMLHttpRequest();
    let name= sessionStorage.getItem("name");;
    
    xhr.open("GET",`/?userName=${name}`);
    xhr.send();
    xhr.onreadystatechange = write;

    function write(){
        if(xhr.status===200&&xhr.readyState===4){
            if(xhr.responseText==="True"){
                document.getElementsByTagName("span")[0].textContent=`Пользователь "${name}" есть`;
            }
            else{
                document.getElementsByTagName("span")[0].textContent=`Пользователя "${name}" нет`;
            };
        }
    }
        
}


