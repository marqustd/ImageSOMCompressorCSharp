// Generated by Hybridizer version 1.2.10341
 #include "cuda_runtime.h"                                                                                  
 #include "device_launch_parameters.h"                                                                      
                                                                                                              
 #if defined(__CUDACC__)                                                                                      
 	#ifndef hyb_device                                                                                       
 		#define hyb_inline __forceinline__                                                                   
 		                                                                                                     
 		#define hyb_constant __constant__                                                                    
 		#if defined(HYBRIDIZER_NO_HOST)                                                                      
 			#define hyb_host                                                                                 
 			#define	hyb_device  __device__                                                                   
 		#else                                                                                                
 			#define hyb_host __host__                                                                        
 			#define	hyb_device  __device__                                                                   
 		#endif                                                                                               
 	#endif                                                                                                   
 #else                                                                                                        
 	#ifndef hyb_device                                                                                       
 		#define hyb_inline inline                                                                            
 		#define hyb_device                                                                                   
 		#define hyb_constant                                                                                 
 	#endif                                                                                                   
 #endif                                                                                                       
                                                                                                              
                                                                                                  
 #if defined _WIN32 || defined _WIN64 || defined __CYGWIN__                                                   
   #define BUILDING_DLL                                                                                       
   #ifdef BUILDING_DLL                                                                                        
     #ifdef __GNUC__                                                                                          
       #define DLL_PUBLIC __attribute__ ((dllexport))                                                         
     #else                                                                                                    
       #define DLL_PUBLIC __declspec(dllexport) // Note: actually gcc seems to also supports this syntax.     
     #endif                                                                                                   
   #else                                                                                                      
     #ifdef __GNUC__                                                                                          
       #define DLL_PUBLIC __attribute__ ((dllimport))                                                         
     #else                                                                                                    
       #define DLL_PUBLIC __declspec(dllimport) // Note: actually gcc seems to also supports this syntax.     
     #endif                                                                                                   
   #endif                                                                                                     
   #define DLL_LOCAL                                                                                          
 #else                                                                                                        
   #if __GNUC__ >= 4                                                                                          
     #define DLL_PUBLIC __attribute__ ((visibility ("default")))                                            
     #define DLL_LOCAL  __attribute__ ((visibility ("hidden")))                                             
   #else                                                                                                      
     #define DLL_PUBLIC                                                                                       
     #define DLL_LOCAL                                                                                        
   #endif                                                                                                     
 #endif                                                                                                       


#if CUDART_VERSION >= 9000
#include <cooperative_groups.h>
#endif
// hybridizer core types
#include <cstdint>
namespace hybridizer { struct hybridobject ; }
namespace hybridizer { struct runtime ; }

#pragma region defined enums and types
