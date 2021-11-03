namespace FunctionalUtilities
open System

module Calculate =
   let levenshtein (s1:string) (s2:string) : int =
        let strOne = s1.ToCharArray ()
        let strTwo = s2.ToCharArray ()
 
        let (distArray : int[,]) = Array2D.zeroCreate (strOne.Length + 1) (strTwo.Length + 1)
 
        for i = 0 to strOne.Length do distArray.[i, 0] <- i
        for j = 0 to strTwo.Length do distArray.[0, j] <- j
 
        for j = 1 to strTwo.Length do
            for i = 1 to strOne.Length do
                if strOne.[i - 1] = strTwo.[j - 1] then distArray.[i, j] <- distArray.[i - 1, j - 1]
                else
                    distArray.[i, j] <- List.min (
                        [distArray.[i-1, j] + 1; 
                        distArray.[i, j-1] + 1; 
                        distArray.[i-1, j-1] + 1]
                    )
        distArray.[strOne.Length, strTwo.Length]

module Transform =
    let singleWhitespace (text: string) =
        let rec singleWhitespace' text' result =
            match text', result with
            | [], result' -> result' |> List.toArray |> String
            | ' '::rt, ' '::_ -> singleWhitespace' rt result 
            | ft::rt, result' -> singleWhitespace' rt (ft::result')
        singleWhitespace' (text |> Seq.toList |> List.rev) []
    
    let removePunctuation (text: string) =
        (text
        |> String.filter (fun c -> Char.IsLetterOrDigit(c) || Char.IsWhiteSpace(c))
        |> singleWhitespace).ToLower()
        
